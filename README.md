
## About

This repository does 3 things:

1. Pre-processing fingerprint images into skeletons
2. Categorizing them (under progress)
3. Matching them (under progress)

Note that this repository is totally an experimental project, developed by a naive 18 y/o. The goal is to get the best results out of every of those 3 things. 

## How to use

*I'll make a cli later.*

## Directories

If you're really into the source code, here's the hierarchy:

- `./Algorithm`: Algorithms for general purposes.
- `./DataStructure`: Data Structures for general purposes.
- `./MatTool`: Classes that handle operations on matrices. Providing 2D Forward Loops, mathematic operators on matrices, matrix converters and more.
- `./Util`: Classes that are specialized in processing Fingerprint and Fingerprint only. Currently includes:
    - `./Util/Preprocessing`: Pre-process the input images into ridges' skeleton
    - `./Util/Singularity`: Extracts key points from fingerprint images
    - `./Util/Comparator`: Compare fingerprints
- `./_dat`: Local data files including input images, processed images, images' encoded data and any data which this program processes. *(This directory is ignored by default)*
- `./_log`: Local log files, mostly used for reports and debugging. *(This directory is ignored by default)*

Since **Speed** and **Memory Efficiency** are highly prioritized (second only to **Accuracy**), this project's implementations and the algorithms behind them are **absolutely incomprehensible** to anyone without documentation (except me, ofc). Therefore, I'll try to write as many docs as I can to explain my code, I promise.
