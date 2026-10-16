# Workaround for upstream saucer bug: the Qt backend creates a brand new
# `QWebEngineProfile("saucer")` for every webview, and IgniteView gives every
# window the same `storage_path`. Chromium does not support two live profiles
# on one data directory: the second profile's navigation never completes (blank
# window, blank devtools, no renderer process) and the browser process keeps
# allocating until the system runs out of memory.
#
# This script makes the Qt backend reuse one profile per storage path, which is
# how WebView2 (shared user data folder) and WKWebView already behave:
#   * `native::profile` becomes a shared_ptr, so the profile lives until the
#     last webview using it is destroyed (pages are always destroyed first).
#   * Only a newly created profile is configured; later webviews reuse it as is.
#   * State that saucer kept on the profile but is really per webview moves to
#     the page: the request interceptor and force-dark setting.
#
# Known limitation: custom scheme handlers are still installed on the profile,
# so two webviews registering the same scheme would clash. IgniteView doesn't
# register any.
#
# Invoked as: cmake -DSAUCER_SRC=<path> -P patch_saucer_qt_shared_profile.cmake

if(NOT DEFINED SAUCER_SRC)
    message(FATAL_ERROR "SAUCER_SRC must be defined")
endif()

set(_marker "IgniteView: shared profile")

# saucer source files contain semicolons, so needles and replacements are always
# passed as single named arguments and never go through CMake lists.

# Reads a saucer source file into `_contents`; sets `_skip` if it is missing or already patched.
macro(_read_source _relative)
    set(_file "${SAUCER_SRC}/${_relative}")
    set(_skip FALSE)
    set(_patch_failed FALSE)

    if(NOT EXISTS "${_file}")
        message(WARNING "patch_saucer_qt_shared_profile: ${_file} not found; saucer source may have changed")
        set(_skip TRUE)
    else()
        file(READ "${_file}" _contents)
        string(FIND "${_contents}" "${_marker}" _already)
        if(NOT _already EQUAL -1)
            set(_skip TRUE)
        endif()
    endif()
endmacro()

# Replaces every occurrence of a needle in `_contents`; flags a failure if it is absent.
function(_replace_in_source _needle _replacement)
    if(_patch_failed)
        return()
    endif()

    string(FIND "${_contents}" "${_needle}" _found)
    if(_found EQUAL -1)
        message(WARNING "patch_saucer_qt_shared_profile: could not locate '${_needle}' in ${_file}; saucer source may have changed")
        set(_patch_failed TRUE PARENT_SCOPE)
        return()
    endif()

    string(REPLACE "${_needle}" "${_replacement}" _contents "${_contents}")
    set(_contents "${_contents}" PARENT_SCOPE)
endfunction()

# Writes `_contents` back, but only if every replacement in the file succeeded.
macro(_write_source)
    if(NOT _patch_failed)
        file(WRITE "${_file}" "${_contents}")
        message(STATUS "patch_saucer_qt_shared_profile: patched ${_file}")
    endif()
endmacro()

_read_source("private/saucer/qt.webview.impl.hpp")
if(NOT _skip)
    _replace_in_source(
        "std::unique_ptr<QWebEngineProfile> profile;"
        "std::shared_ptr<QWebEngineProfile> profile; // ${_marker} between webviews with the same storage path")
    _write_source()
endif()

_read_source("src/qt.webview.cpp")
if(NOT _skip)
    # Look the profile up by storage path, and only configure it when it is new.
    # The block opened here is closed after the last profile setting below.
    _replace_in_source(
        "auto profile = std::make_unique<QWebEngineProfile>(\"saucer\");"
        "// ${_marker}: Chromium can't run two profiles on one data directory, so reuse one per storage path.\n        static std::unordered_map<std::string, std::weak_ptr<QWebEngineProfile>> shared_profiles;\n\n        const auto profile_key = opts.storage_path.has_value() ? opts.storage_path->string() : std::string{};\n        auto profile           = shared_profiles[profile_key].lock();\n\n        if (!profile)\n        {\n        profile                      = std::make_shared<QWebEngineProfile>(\"saucer\");\n        shared_profiles[profile_key] = profile;\n")
    _replace_in_source(
        "profile->settings()->setAttribute(QWebEngineSettings::FullScreenSupportEnabled, true);"
        "profile->settings()->setAttribute(QWebEngineSettings::FullScreenSupportEnabled, true);\n        } // end of new-profile configuration")
    # Force dark is a per window setting, so keep it on the page instead of the shared profile.
    _replace_in_source(
        "platform->profile->settings()"
        "platform->web_page->settings()")
    _write_source()
endif()

_read_source("src/qt.webview.impl.cpp")
if(NOT _skip)
    _replace_in_source(
        "profile->setUrlRequestInterceptor(interceptor.get());"
        "web_page->setUrlRequestInterceptor(interceptor.get()); // ${_marker}, so intercept per page")
    _write_source()
endif()
