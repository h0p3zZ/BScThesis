# BScThesis

This repository contains the Bachelor's Thesis of Daniel Wimmer.

# Notes
It has Notes, which are basic markdown files containing the thought process behind the development of the thesis in addition to the notes for the meetings with the thesis supervisor. 

# Tex
We have the `_tex` folder, which contains the final thesis in LaTeX format. 

# Prog
The `_prog` folder contains the programming parts of the thesis. The programs are written in `c#` `.NET10`.\
It is split into to c# projects:
* JsonVerfier that verifies the integrity of the problem-definition-JSON
* SolutionVerifier in which a solution can be verified and a corresponding cost is computed depending on the solution and problem definitions.

## Running the projects
```sh
./_prog/JsonVerifier> dotnet run <path_to_json>
```

```sh
./_prog/SolutionVerifier> dotnet run <problem-file-path> <assignment_file_path>
```

## Running tests
```sh
./_prog/JsonVerifier> dotnet test
```

```sh
./_prog/SolutionVerifier> dotnet test
```
