(*
 * The MIT License (MIT)
 *
 * Copyright (c) 2014 Andrew B. Johnson
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 *
 * Scan for any directories in the provided directory (or CD by default) for any subdirectories that don't follow the Plex TV structure 
 *)

open System
open System.IO

let getSubfolders folder = Directory.EnumerateDirectories folder |> Seq.cast<string>

let folderEquals (targetName: string) folder =
  let folderName = Path.GetFileName(folder)
  targetName.Equals(folderName, StringComparison.OrdinalIgnoreCase)

let doesFolderHaveUnaccountedFolders folder = 
  let extraFolders = query {
    for f in getSubfolders folder do
    where (not (folderEquals "Extras" f || folderEquals "Specials" f || Path.GetFileName(f).IndexOf("Season") <> -1))
    select f
  }

  not <| Seq.isEmpty extraFolders

let verifyFolder folder _ = not <| doesFolderHaveUnaccountedFolders folder

let scanFolder rootFolder =
  getSubfolders rootFolder
  |> Seq.map (fun f -> (f, false))
  |> Map.ofSeq
  |> Map.map verifyFolder

let processFolder folder = 
  printf "Scanning folder %s\n" folder |> ignore
  match Directory.Exists folder with
  | true -> scanFolder folder
  | false -> Map.empty

let printResults results =
  let printResult key value =
    match value with
    | false -> printf "%s\n" key |> ignore
    | true -> ()

  Map.iter printResult results

[<EntryPointAttribute>]
let main args =
  printf "Scanning Folders\n" |> ignore
  let folder = Array.tryHead args
  match folder with
  | Some(x) -> processFolder x
  | None -> processFolder <| Directory.GetCurrentDirectory()
  |> printResults
  |> ignore
  0