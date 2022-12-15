
## (Not an) Overview

The refined version of my [Fingerprint Recognition](https://github.com/iluvgirlswithglasses/FingerprintRecognition) project

Only the neat and optimized codes should present here.

This README.md will see a huge update & documentation later.

## Directories

- `./_dat`: Local data files including input images, processed images, images' encoded data and any data which this program processes. *(This directory is ignored by default)*
- `./_log`: Local log files, mostly used for reports and debugging. *(This directory is ignored by default)*
- `./Algorithm`: Algorithms for general purposes.
- `./DataStructure`: Data Structures for general purposes.
- `./MatTool`: Classes that handle operations on matrices. Providing 2D Forward Loops, mathematic operators on matrices, matrix converters and more.
- `./Util`: Classes that are specialized in processing Fingerprint and Fingerprint only. Currently includes:
    - `./Util/Preprocessing`: Pre-process the input images into ridges' skeleton
    - `./Util/Singularity`: Extracts key points from fingerprint images
    - `./Util/Comparator`: Compare fingerprints
