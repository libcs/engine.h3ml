cmake_minimum_required(VERSION 3.8.0)
project(gumbo)

# WINDOWS
if(WIN32)
   # install vcpkg
   if (NOT IS_DIRECTORY _vcpkg)
      message("vcpkg does not exist. installing...")
      execute_process(
         COMMAND git.exe clone https://github.com/Microsoft/vcpkg.git _vcpkg
         WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}
      )
      execute_process(
         COMMAND bootstrap-vcpkg.bat
         WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}/_vcpkg
      )
   endif()

   # install gumbo:x86-windows
   if (NOT IS_DIRECTORY _vcpkg/packages/gumbo_x86-windows)
      execute_process(
         COMMAND vcpkg install gumbo:x86-windows
         WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}/_vcpkg
      )
      # generate gumbo.def
      execute_process(
         COMMAND dotnet run --project ./Gumbo.DefGen x86 _vcpkg/packages/gumbo_x86-windows/lib/gumbo.lib
         WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}
      )
   endif()

   # install gumbo:x64-windows
   if (NOT IS_DIRECTORY _vcpkg/packages/gumbo_x64-windows)
      execute_process(
         COMMAND vcpkg install gumbo:x64-windows
         WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}/_vcpkg
      )
      # generate gumbo.def
      execute_process(
         COMMAND dotnet run --project ./Gumbo.DefGen x64 _vcpkg/packages/gumbo_x64-windows/lib/gumbo.lib
         WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}
      )
   endif()
endif()

set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_HOME_DIRECTORY}/_build)