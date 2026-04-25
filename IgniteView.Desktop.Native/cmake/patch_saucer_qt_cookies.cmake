# Workaround for upstream saucer bug: the Qt backend constructs `QWebEnginePage`
# without passing the persistent profile, so on Linux the page silently falls
# back to Qt's default off-the-record profile. This causes
# `persistent_cookies = true` and `storage_path` to be ignored, and cookies are
# lost between runs.
#
# Regression: saucer commit 36736c4 ("fix(qt6): segfault"), still present on
# master and in tag v8.0.5.
#
# This script rewrites the offending line in `src/qt.webview.cpp` to pass the
# profile pointer to the page constructor (matching the pre-regression
# behaviour from commit eee30db).
#
# Invoked as: cmake -DSAUCER_SRC=<path> -P patch_saucer_qt_cookies.cmake

if(NOT DEFINED SAUCER_SRC)
    message(FATAL_ERROR "SAUCER_SRC must be defined")
endif()

set(_file "${SAUCER_SRC}/src/qt.webview.cpp")

if(NOT EXISTS "${_file}")
    # Nothing to patch (e.g. saucer hasn't been fetched yet, or layout changed).
    return()
endif()

file(READ "${_file}" _contents)

set(_needle  "platform->web_page    = std::make_unique<QWebEnginePage>();")
set(_replace "platform->web_page    = std::make_unique<QWebEnginePage>(platform->profile.get()); // IgniteView: pass profile so persistent cookies/storage actually take effect")

string(FIND "${_contents}" "${_replace}" _already)
if(NOT _already EQUAL -1)
    return()
endif()

string(FIND "${_contents}" "${_needle}" _found)
if(_found EQUAL -1)
    message(WARNING "patch_saucer_qt_cookies: could not locate target line in ${_file}; saucer source may have changed")
    return()
endif()

string(REPLACE "${_needle}" "${_replace}" _contents "${_contents}")
file(WRITE "${_file}" "${_contents}")
message(STATUS "patch_saucer_qt_cookies: patched ${_file} to attach saucer profile to QWebEnginePage")
