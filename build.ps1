properties {
    $root =                 $psake.build_script_dir
    $solution_file =        "$root/src/Nancy.Raygun.sln"
    $configuration =        "Debug"
    $build_dir =            "$root\build\"
    $release_dir =          "$root\release\"
    $tools_dir =            "$root\tools"
    $env:Path +=            ";$tools_dir"
    $assemblies_to_merge =  "Nancy.Raygun.dll"
    $merged_assemlby_name = "Nancy.Raygun.dll"
}

task default -depends Merge

task Clean {
    remove-item -force -recurse $build_dir -ErrorAction SilentlyContinue | Out-Null
    remove-item -force -recurse $release_dir -ErrorAction SilentlyContinue | Out-Null
}

task Init -depends Clean {
    new-item $release_dir -itemType directory | Out-Null
    new-item $build_dir -itemType directory | Out-Null
}

task Compile -depends Init {
    exec { msbuild "$solution_file" /m /p:OutDir=$build_dir /p:Configuration=$configuration }
}

task Merge -depends Compile {
    Push-Location -Path $build_dir

    exec { ilmerge.exe /internalize /targetplatform:v4 /out:"$release_dir\$merged_assemlby_name" $assemblies_to_merge }

    Pop-Location
}
