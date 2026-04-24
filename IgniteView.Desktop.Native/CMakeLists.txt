cmake_minimum_required(VERSION 3.25)
project(IgniteView.Desktop.Native LANGUAGES CXX C)

if(APPLE)
    enable_language(OBJCXX)
endif()

add_library(IgniteView.Desktop.Native SHARED)

target_compile_features(${PROJECT_NAME} PRIVATE cxx_std_23)
set_target_properties(${PROJECT_NAME} PROPERTIES CXX_STANDARD 23 CXX_EXTENSIONS OFF CXX_STANDARD_REQUIRED ON)

if(UNIX AND NOT APPLE)
    set(saucer_backend "Qt6")
endif()

set(saucer_serializer "Rflpp")
set(CMAKE_POSITION_INDEPENDENT_CODE ON)

include(FetchContent)

FetchContent_Declare(
  saucer
  GIT_REPOSITORY https://github.com/saucer/saucer
  GIT_TAG        8ace837fbf6ce15e0a3aed45fa0d3e2b735110e8
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
