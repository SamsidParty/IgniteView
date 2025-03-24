cmake_minimum_required(VERSION 3.25)
project(IgniteView.Desktop.Native LANGUAGES CXX C)

add_library(IgniteView.Desktop.Native SHARED)

target_compile_features(${PROJECT_NAME} PRIVATE cxx_std_23)
set_target_properties(${PROJECT_NAME} PROPERTIES CXX_STANDARD 23 CXX_EXTENSIONS OFF CXX_STANDARD_REQUIRED ON)

set(saucer_serializer "Rflpp")
set(saucer_polyfills "None")
set(CMAKE_POSITION_INDEPENDENT_CODE ON)

include(FetchContent)

FetchContent_Declare(
    saucer 
    GIT_REPOSITORY "https://github.com/SamarthMP/saucer" 
    GIT_BRANCH master
)

FetchContent_MakeAvailable(saucer)

target_sources(IgniteView.Desktop.Native PUBLIC Main.cpp MacHelper.h)
target_link_libraries(${PROJECT_NAME} PUBLIC saucer::saucer)