# Xenia Deck

<p align="center">
    <a href="https://github.com/Xenia-Deck/Xenia-Deck/actions/workflows/staging-flight.yml">
        <img src="https://github.com/Xenia-Deck/Xenia-Deck/actions/workflows/staging-flight.yml/badge.svg"
             alt="Deploy Staging Flight">
    </a>
    &nbsp;&nbsp;
    <a href="https://github.com/Xenia-Deck/Xenia-Deck/actions/workflows/master-prod.yml">
        <img src="https://github.com/Xenia-Deck/Xenia-Deck/actions/workflows/master-prod.yml/badge.svg"
             alt="Deploy Production">
    </a>
</p>

A modern, Fluent-designed Xenia Emulator library manager built with WinUI 3 for seamless Xbox 360 emulation setup on Windows 10/11. Available on the Microsoft Store.

<p align="center">
    <a href="https://apps.microsoft.com/detail/9P3M272MD942?referrer=appbadge&mode=full">
        <picture>
            <source media="(prefers-color-scheme: dark)"
                    srcset="https://get.microsoft.com/images/en-us%20light.svg">
            <img src="https://get.microsoft.com/images/en-us%20dark.svg"
                 width="200"
                 alt="Get it from Microsoft">
        </picture>
    </a>
</p>

## Screenshots

![Game library](https://store-images.s-microsoft.com/image/apps.54391.14535789018900745.43fd22b3-63d8-4253-9a5a-a795900bcf4e.e33cdc9f-d2e7-4386-a9a1-a9505b18f7ae)

![Manage Xenia](https://store-images.s-microsoft.com/image/apps.6766.14535789018900745.43fd22b3-63d8-4253-9a5a-a795900bcf4e.f61ffde7-da97-4847-8590-4656f149b180)

## How to build and run

Quick start guide to building and running Xenia Deck on your machine.

### 1. Set up your development environment

Visual Studio 2026 is required to build and run Xenia Deck. Make sure you have the WinUI application development workload installed.

### 2. Clone the repository

```
git clone https://github.com/Xenia-Deck/Xenia-Deck.git
```

### 3. Build and run

Open the solution file `XeniaDeck.sln` in Visual Studio 2026. Set the startup project to `XeniaDeck` and build the solution. Once built, you can run the application directly from Visual Studio.