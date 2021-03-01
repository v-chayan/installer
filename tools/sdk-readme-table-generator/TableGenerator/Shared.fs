﻿module TableGenerator.Shared

open NuGet.Versioning
open System

type Branch =
    { GitBranchName: string
      DisplayName: string
      AkaMsChannel: string option 
      UseLegacyAkaMsTemplate: bool }

type ReferenceTemplate =
    { AkaMSTemplate: string
      LegacyTemplate: string 
      AkaMSLegacyTemplate: string}

let branchNameShorten (branch: Branch): string =
    branch.GitBranchName.Substring(branch.GitBranchName.IndexOf('/') + 1).Replace("xx", "XX")

type BranchMajorMinorVersion =
    { Major: int
      Minor: int
      Patch: int
      Release: string}

type BranchMajorMinorVersionOrMaster =
    | Master
    | MajorMinor of BranchMajorMinorVersion
    | NoVersion

let getMajorMinor (branch: Branch): BranchMajorMinorVersionOrMaster =
    match branch.GitBranchName = "master" with
    | true -> Master
    | _ ->
        match branch.GitBranchName.IndexOf('/') with
        | index when index < 0 -> NoVersion
        | _ ->
            match NuGetVersion.TryParseStrict
                      (branch.GitBranchName.Substring(branch.GitBranchName.IndexOf('/') + 1).Replace("xx", "99")) with
            | true, version ->
                MajorMinor
                    { Major = version.Major
                      Minor = version.Minor
                      Patch = version.Patch
                      Release = version.Release}
            | _ -> NoVersion
