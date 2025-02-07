cmake_minimum_required(VERSION 3.25)
project(IgniteView.Desktop.Native)
add_library(IgniteView.Desktop.Native SHARED)

set(CMAKE_RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/bin")
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/lib")
set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/lib")

include(FetchContent)

FetchContent_Declare(
    webview
    GIT_REPOSITORY https://github.com/webview/webview
    GIT_TAG 0.12.0)
FetchContent_MakeAvailable(webview)

target_sources(IgniteView.Desktop.Native PRIVATE Main.cpp)
target_link_libraries(IgniteView.Desktop.Native PUBLIC webview::core)