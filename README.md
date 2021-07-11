# UnAuthorised_View



Keys For debugging:

Numeric keys - 1 - 6 play the movie

Function keys (fn F1 - F6) choose the selected version of buttons

W - flags the current movie as watched - regardless of whether it has finished.



State machine:

The experience uses a state machine - which takes a visitor through the experience.

The state machine starts with ExperienceState.ResetExperience and changes to ExperienceState.WaitingForHeadsetToBeWorn.

It leaves ExperienceState.WaitingForHeadsetToBeWorn when a user places the headset on. It then returns to ExperienceState.ResetExperience when the user takes that headset off.

The XR plugin will only detect the presence of the user on the vendor specific drivers so ensure you are running Oculus XR plugin in Build Settings > Player Setings > XR Plug-in Management.
