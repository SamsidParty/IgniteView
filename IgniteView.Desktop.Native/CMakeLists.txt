cmake_minimum_required(VERSION 3.25)
project(IgniteView.Desktop.Native LANGUAGES CXX C)

if(APPLE)
    enable_language(OBJCXX)
endif()

add_library(IgniteView.Desktop.Native SHARED)

target_compile_features(${PROJECT_NAME} PRIVATE cxx_std_23)
set_target_properties(${PROJECT_NAME} PROPERTIES CXX_STANDARD 23 CXX_EXTENSIONS OFF CXX_STANDARD_REQUIRED ON)

if(UNIX AND NOT APPLE)
    # saucer v8 dropped Qt5 support and renamed the `Qt6` backend to `Qt`.
    set(saucer_backend "Qt")
endif()

set(saucer_serializer "Rflpp")
set(CMAKE_POSITION_INDEPENDENT_CODE ON)

include(FetchContent)

FetchContent_Declare(
  saucer
  GIT_REPOSITORY https://github.com/saucer/saucer
  GIT_TAG        v8.0.5
  # Workaround for upstream saucer bug where the Qt backend constructs
  # QWebEnginePage without the persistent profile, breaking persistent cookies
  # and storage on Linux. See cmake/patch_saucer_qt_cookies.cmake.
  PATCH_COMMAND  ${CMAKE_COMMAND}
                 -DSAUCER_SRC=<SOURCE_DIR>
                 -P ${CMAKE_CURRENT_SOURCE_DIR}/cmake/patch_saucer_qt_cookies.cmake
  UPDATE_DISCONNECTED TRUE
)

FetchContent_MakeAvailable(saucer)

target_sources(IgniteView.Desktop.Native PUBLIC Main.cpp MacHelper.h)

if(APPLE)
    target_sources(IgniteView.Desktop.Native PRIVATE MacHelper.mm)
    # Main.cpp pulls in saucer's webkit module header on Apple, which transitively
    # includes Objective-C types (NSString, etc). Compile it as Objective-C++.
    set_source_files_properties(Main.cpp PROPERTIES LANGUAGE OBJCXX)
endif()

target_link_libraries(${PROJECT_NAME} PUBLIC saucer::saucer)
# saucer v8 moved private headers (e.g. the NuGet WebView2.h bundled with the
# Windows build, the Qt includes resolved by saucer's CPM config, and the Cocoa
# bits on macOS) behind the `saucer::private` target. The stable module headers
# we include from Main.cpp (`saucer/modules/stable/webview2.hpp`, `.../webkit.hpp`)
# drag in those platform SDK headers, so we must link the private target to
# inherit the include paths.
target_link_libraries(${PROJECT_NAME} PRIVATE saucer::private)
