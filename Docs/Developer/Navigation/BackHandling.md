# Back handling

## General

Back button is handled in two ways in Omega Go. The system handles common navigation patterns on all devices - title back button on Desktop, back button on Mobile, B button on Xbox, virtual back button on HoloLens and also hardware back keys on mouse or keyboard. 
These events all surface as the `SystemNavigationManager.GetForCurrentView().BackRequested` event.
Omega Go also wires up the Esc key as it is quite common for apps to use it for back navigation.

Both back button handling paths are handled by the `AppShell` which also allows you to trigger back navigation manually using the `GoBack` method.

The logic then checks if the app frame can navigate back and then gives the control to the current ViewModel.

## Prevent back navigation in ViewModel

You can prevent back navigation in the ViewModel by overriding the `GoBack` method. This method is virtual and is called both by the `AppShell` on Back navigation request and the `GoBackCommand` in the `ViewModelBase`.

If you override the `GoBack` method, you can display a confirmation dialog to the user or otherwise alter the back navigation flow. The `ViewModelBase` implementation simply calls `Close( this )` to exit the current ViewModel.