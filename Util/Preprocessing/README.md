
# Summary

This module proposes a fingerprint pre-processing chain based on generic algorithms for filtering images.

# How to use

Simply load an `Emgu.CV.Image<Gray, byte>` object to a new `ProcImg` object, then retrieve the results via the object public fields & `ProcImg` static fields. Like this, for example:

```C#
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

By this transformation, finger pressure differences are removed.

### 2 - Segmentation

This step provides a mask which determines whether a region in the image is a part of the fingerprint, or it's just background. The calculation is performed solely based on the given standard deviation, which might result in false recognition due to heavy noises. Such as this case:

Source                     | Result                    | Noise                     |
:-------------------------:|:-------------------------:|:-------------------------:|
![235-src](https://user-images.githubusercontent.com/58514512/211996283-66d5f78e-625f-4a78-bb33-11946da40cde.png) | ![235-res](https://user-images.githubusercontent.com/58514512/211996302-8fdc35be-3be1-4a96-a922-a41be72bf799.png) | ![235-noise](https://user-images.githubusercontent.com/58514512/211996651-bc09f6e3-871e-4586-a36b-f07cd234edc9.png) |

Sometimes, there're some regions of a fingerprint that contain no useful information at all, but is still included in the mask because it's a ridge anyway. The elimination of these redundant ridges shall be performed later, when the program has extracted more information from the image.

### 3 - Orientation

Calculates the ridges' orientation in every given region. The result of this step is crucial to the Gabor filter & the singularities detection step.

### 4 - The Second Normalization

Modifies the initial image into an image that has the standard deviation of its fingerprint regions equals to 1, using the mask obtained in Step 2 - Segmentation.

### 5 - Wavelength Calculation

Gets the ridges' median wavelength.

### 6 - Gabor Filter

As it says, this step produces a Gabor Image. See more about Gabor Filter [here](https://en.wikipedia.org/wiki/Gabor_filter).

### 7 - Thinning

Achieves the skeleton of the Gabor Image using [Zhang-Suen](https://rosettacode.org/wiki/Zhang-Suen_thinning_algorithm) thinning technique.

# Implementation

In this section, I'll explain how I implemented each of those aforementioned steps. Since performance is heavily prioritized over readability, no one would understand a sh!t looking at those codes without documentation. So here it goes

*i'm still writing*
