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
 * Move all folders to the "Extras" folder for further processing
 *)

open System;
open System.IO;

let getSubfolders folder = Directory.EnumerateDirectories folder |> Seq.cast<string>

let folderEquals (targetName: string) folder =
  let folderName = Path.GetFileName(folder)
  targetName.Equals(folderName, StringComparison.OrdinalIgnoreCase)

let getFoldersThatNeedProcessing folder = 
  query {
    for f in getSubfolders folder do
    let k = printf "Index of Season: %i\n" (f.IndexOf("Season"))
    where (not (folderEquals "Extras" f || folderEquals "Specials" f || Path.GetFileName(f).IndexOf("Season") <> -1))
    select f
  }

let moveAllSeasonFoldersOutOfExtrasFolder folder = 
  let moveFolder targetFileToMove =
    let finalPath = Path.Combine(folder, Path.GetFileName(targetFileToMove))
    printf "Moving %s -> %s\n" targetFileToMove finalPath
    Directory.Move(targetFileToMove, finalPath)
  let extrasFolderPath = Path.Combine(folder, "Extras")
  match Directory.Exists(extrasFolderPath) with
  | true ->
    Directory.EnumerateDirectories extrasFolderPath
    |> Seq.cast<string>
    |> Seq.where (fun f ->  Path.GetFileName(f).IndexOf("Season") <> -1)
    |> Seq.iter (fun f -> moveFolder f)
  | false -> ()
  |> ignore

let moveAllElementsIntoExtrasFolder folder = 
  let extrasFolderPath = Path.Combine(folder, "Extras")
  match Directory.Exists(extrasFolderPath) with
  | true -> ()
  | false ->
    printf "Creating Extras folder: %s" extrasFolderPath |> ignore
    Directory.CreateDirectory(extrasFolderPath) |> ignore
  |> ignore
  let moveNoOp targetFolder =
    let nameOfFolder = Path.GetFileName(targetFolder)
    let finalPath = Path.Combine(extrasFolderPath, nameOfFolder)
    printf "Moving %s -> %s\n" targetFolder finalPath
    Directory.Move(targetFolder, finalPath)

  getFoldersThatNeedProcessing folder
  |> Seq.iter moveNoOp

let processRootFolder rootFolder =
  getSubfolders rootFolder
  // |> Seq.iter moveAllElementsIntoExtrasFolder
  // |> Seq.iter moveAllSeasonFoldersOutOfExtrasFolder
  |> ignore

[<EntryPointAttribute>]
let main args =
  let folder = Array.tryHead args
  match folder with
  | Some(x) -> processRootFolder x
  | None -> raise (InvalidOperationException("Must provide a root folder to process"))
  |> ignore
  0