# Hui Manu o Ku – Aloha Birds Game

A mobile augmented reality (AR) Unity game built on Niantic Lightship, aiming to educate and engage users with Hawaii's native bird population.

## Table of Contents

- [Hui Manu o Ku – Aloha Birds Game](#hui-manu-o-ku--aloha-birds-game)
  - [Table of Contents](#table-of-contents)
  - [About the Project](#about-the-project)
  - [Features](#features)
  - [Technical Features](#technical-features)
    - [1. Login Screen](#1-login-screen)
    - [2. Map Scene](#2-map-scene)
    - [3. Augmented Reality Scene](#3-augmented-reality-scene)
    - [4. Avidex](#4-avidex)
    - [5. Profile Screen](#5-profile-screen)
    - [6. AI Recognition Tool](#6-ai-recognition-tool)
  - [Future Features](#future-features)
  - [Requirements](#requirements)
  - [Installation](#installation)
  - [Running the apk (Android)](#running-the-apk-android)
  - [Running the iOS Developer Mode (iOS)](#running-the-ios-version-in-developer-mode)

## About the Project

**Hui Manu o Ku – Aloha Birds Game** is an AR experience that combines exploration and education. Users can discover virtual representations of Hawaii's native birds in their natural habitats, learn about their behaviors, and understand the importance of conservation efforts.

## Features

- **Augmented Reality Exploration**: Discover and interact with virtual birds in real-world locations.
- **Educational Content**: Learn about each bird species through informative descriptions and audio cues.
- **Interactive Map**: Navigate through different habitats and track discovered species.
- **Cross-Platform Support**: Available on both Android and iOS devices.

## Technical Features
### 1. Login Screen
   - Users enter a unique username, securely stored locally to retrieve data in future sessions.
### 2. Map Scene
   - After login, users are directed to a real-time map centered on their current location.
   - Virtual bird pins, placed based on GPS data, represent real birds in Hawaii.
   - Users can rotate the camera view to explore surrounding birds in the environment.
### 3. Augmented Reality Scene
   - Tapping on a bird pin transports users to an AR scene with a 3D animated bird model.
   - Users can capture an image of the bird, which is saved to their personal gallery called the **Avidex**.
### 4. Avidex
   - A compendium of captured birds accessible from the map screen.
   - Users can browse bird entries, view facts, and revisit captured images.
### 5. Profile Screen
   - Displays user stats such as username, unique birds captured, total points, and bird capture count.
   - Includes an **AI Recognition Tool** allowing users to upload images of unidentified birds for automatic identification.
### 6. AI Recognition Tool
   - Identifies bird species from user-uploaded photos.
   - Provides a brief description of the bird for users to learn more about it.

## Future Features
Given more time, we plan to add:
* **Multiplayer and Leaderboard**: Compare points and bird captures with others.
* **Shared AR**: Allowing multiple users to see the same virtual birds in real-time.
* **Enhanced Bird Spawning Logic**: Birds appear based on time of day, rarity, and region.
* **Profile Customization**: Personalized avatars and profile images.

## Requirements

- **Unity**: Version 2022.3.50f1
- **Niantic Lightship ARDK**: Installed in Unity
- **Xcode(iOS)**: For building and deploying to iOS devices
- **Lightship API Key**: Required for ARDK functionalities
- **OpenAI API Key**: Required for ChatGPT integration
- **Mobile Device**: Android or iOS device for testing AR features
- **Visual Studio**: For modifying Unity C# Scripts

## Installation

1. **Clone the Repository**

   ```bash
   git clone https://github.com/your-username/hui-manu-o-ku-aloha-birds-game.git```  


2. **Download and Install Unity Hub and Unity Editor**

3. **Open one of the scenes in Unity Editor**
4. **Open Build Settings and Deploy To Your Device**


## Running the apk (Android)
1. Clone the repo and open the AlohaBirdsGame.apk file in the **Builds** folder and launch it on your android device, the app should run successfully

## Running the iOS Version in Developer Mode
Connect your iPhone to the Mac computer.
1. In Unity, enter your Lightship API key in Niantic Lightship SDK field located in Project Settings on Unity.
2. Follow unity tutorial steps linked here for deploying and working with Xcode: [Unity Tutorial iOS Instructions](https://learn.unity.com/tutorial/deploy-your-project-to-ios-or-android?pathwayId=63e3a4c1edbc2a344bfe21d8&missionId=63f63a3bedbc2a663dc6ffde#633d41a7edbc2a4bb08558b8)

Thank you for reviewing our project!. :hibiscus:
