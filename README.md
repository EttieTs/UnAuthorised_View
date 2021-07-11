# UnAuthorised_View



##  Keys For debugging

Numeric keys - 1 - 6 play the movie

Function keys (fn F1 - F6) choose the selected version of buttons

W - flags the current movie as watched - regardless of whether it has finished.



##  State machine

The experience uses a state machine - which takes a visitor through the experience.

The state machine starts with ExperienceState.ResetExperience and changes to ExperienceState.WaitingForHeadsetToBeWorn.

It leaves ExperienceState.WaitingForHeadsetToBeWorn when a user places the headset on. It then returns to ExperienceState.ResetExperience when the user takes that headset off.

The XR plugin will only detect the presence of the user on the vendor specific drivers so ensure you are running Oculus XR plugin in Build Settings > Player Setings > XR Plug-in Management.


## Look Around You

We can move states after specific amount of time has passed but this doesn't account for viewers who want to go at their own space. Instead we can track the movement of the headset. Everytime the movement has passed through 30 degrees in any axis we increment a count.

We can append this onto our Look Around You message to debug. But this might allow a future version to have a progress bar that changes as we look around. 

```
#if UNITY_EDITOR
        lookAroundYouText.GetComponent<Text>().text = "Look Around You: " + lookAroundYouCount.ToString();        
#endif
```

## Particle Effect

Particle effect follows tutorial from:

https://www.lmhpoly.com/tutorials/unity-tutorial-falling-leaves-particle-system
https://www.youtube.com/watch?v=wQJ0_TqoLr4
