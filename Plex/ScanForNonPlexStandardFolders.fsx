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

let doesFolderHaveFolderWithName folder name =
  let predicate = fun state (element: string) -> 
    let partialMatch = element.IndexOf(name, StringComparison.OrdinalIgnoreCase)
    match partialMatch, state with 
    | _, true -> true
    | -1, false -> false
    | _, false -> true

  Directory.EnumerateDirectories folder
  |> Seq.cast<string>
  |> Seq.fold predicate false

let doesFolderHaveSpecialsFolder folder = doesFolderHaveFolderWithName "Specials"

let doesFolderHaveSeasonFolders folder = doesFolderHaveFolderWithName "Season"

let verifyFolder folder _ =
  false

let scanFolder rootFolder =
  Directory.EnumerateDirectories rootFolder
  |> Seq.cast<string>
  |> Seq.map (fun f -> (f, false))
  |> Map.ofSeq
  |> Map.map verifyFolder

let processFolder folder = 
  match Directory.Exists folder with
  | true -> scanFolder folder
  | false -> Map.empty

let printResults results =
  None

let main args =
  let folder = Array.tryHead args
  match folder with
  | Some(x) -> processFolder x
  | None -> processFolder <| Directory.GetCurrentDirectory()
  |> printResults
  |> ignore
  0