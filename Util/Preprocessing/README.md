
# Summary

This module proposes a fingerprint pre-processing chain based on generic algorithms for filtering images.

# How to use

Simply load an `Emgu.CV.Image<Gray, byte>` object to a new `ProcImg` object, then retrieve the results via the object public fields & `ProcImg` static fields. Like this, for example:

```
Image<Gray, byte> src = new("data/fingerprint.png");
ProcImg img = new(src);

// print the wavelength of the fingerprint
Console.WriteLine(img.WaveLen);

// the skeleton image is stored in a static field for memory manipulation reasons
CvInvoke.Imwrite("skeleton.png", MatConverter.Bool2Img(ProcImg.SkeletonMat));
```

# Pipeline

### 1 - Image Normalization

The goal of this step is to transform the initial grayscale image into an image that:

- Has the average value of its pixels equals to 0
- Has its standard deviation equals to 1

### 2 - Segmentation

This step provides a mask which determines whether a region in the image is a part of the fingerprint, or it's just background. The calculation is performed solely based on the given standard deviation, which might result in false recognition due to heavy noises... But I'll figure it out later.

### 3 - Orientation

Calculates the ridges' orientation in every given region. The result of this step is crucial to the Gabor filter & the singularities detection step.

### 4 - The Second Normalization

Modifies the initial image into an image that has the standard deviation of its fingerprint regions equals to 1, using the mask obtained in Step 2 - Segmentation.

### 5 - Wavelength Calculation

Gets the ridges' median wavelength.

### 6 - Gabor Filter

As it says, this step produces a Gabor Image. See more about Gabor Filter ![here](wiki).

### 7 - Thinning

Gets the skeleton of the Gabor Image using ![Zhang Suen](wiki) thinning technique.

# Implementation

In this section, I'll explain how I implemented each of those aforementioned steps. Since performance is heavily prioritized over readability, no one would understand a sh!t looking at those codes without documentation. So here it goes

*i'm still writing*
