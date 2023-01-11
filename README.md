
# About

This repository does 3 things:

1. Pre-processing fingerprint images into skeletons
2. Categorizing them (under progress)
3. Matching them (under progress)

Note that this repository is totally an experimental project, developed by a naive 18 y/o. The goal is to get the best results out of every of those 3 things. 

# About the pre-processing module

I'll just leave some results here

Source                     | Gabor Filter               | Skeletonization
:-------------------------:|:-------------------------: | :-------------------------:
![src](https://user-images.githubusercontent.com/58514512/211828914-0f4c6fc9-221e-4397-bd63-eb1320f8c7ad.png) | ![gabor](https://user-images.githubusercontent.com/58514512/211828957-6936eb4a-ebd0-4517-99d3-c6552e7707b3.png) | ![ske](https://user-images.githubusercontent.com/58514512/211829002-0b2057ed-cef8-41ff-93a3-2526128071ed.png)

The keypoints detection is handled by *the matching module*, and the singularities detection is handled by *the categorizing module*. So the output of this pre-processing module are:

- A Gabor image
- A Skeleton image
- A fingerprint orientation image (which hasn't been visualized here)
- And the fingerprint's wavelength

There's still one thing I should improve about this module, which is the **Segmentation**. I'll do it later.

# How to use

*I'll make a cli later.*

Note: This project only processes images with the size of 480 pixels in Height and 320 pixels in Width. Additionally, the ridges of the fingerprint should be white, and the background should be black.

# Directories

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
