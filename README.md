
# Techne

 Techne is a simple-to-use, component-focused electronic circuit simulator created by Chris Beimers.
 Techne can be downloaded [here](Techne.zip).

  

## Controls

- Left-click: Place the selected component onto the board
- Right-click: "Use" a component on the board if the component can take user input (eg: buttons)
- Shift-right-click: Remove a component from the board
- Shift-left-click: Begin a wire which will carry the output of the clicked on component, release on signal destination
- Middle-click: Press and hold to pan the camera around the board
- Shift-middle-click: Focus on a selected component slot
- Scroll wheel: Zoom in and out of the board
- Number keys: Select the relative hotbar component (includes `~ key as first position)
- Control-left-click: Open the inspection menu on a component
  

## Circuit Flow

A circuit must have an OUTPUT component to act as the output of the circuit. The OUTPUT component actively searches for components connected to it and "pulls" the signal through that component and every component coming into it, recursively. Currently, the only way to manually input a signal to a circuit is via a BUTTON component. 

Here is a simple togglable LED circuit to get you started:
 1. Place one of: OUTPUT, LED, BUTTON
 2. Shift+left click and drag to place wires from source to destination. Wire BUTTON -> LED and LED -> OUTPUT
 3. Right-click on the button to toggle the LED 