
# Techne

 Techne is a simple-to-use, component-focused electronic circuit simulator created by Chris Beimers.

  

## Controls

- Left-click on component hotbar: Select which component you'd like to put on the board
- Left-click: Place the selected component onto the board
- Right-click: "Use" a component on the board if the component can take user input (eg: buttons)
- Shift-right-click: Remove a component from the board
- Shift-left-click: Begin a wire which will carry the output of the clicked on component, release on signal destination
- Middle-click: Press and hold to pan the camera around the board
- Shift-middle-click: Focus on a selected component slot
- Scroll wheel: Zoom in and out of the board
- Shift-scroll-wheel: Scroll through the component hotbar
- A: Select the hotbar component to the left of the current component
- D: Select the hotbar component to the right of the current component
- Number keys: Select the relative hotbar component (includes `~ key as first position)

  

## Circuit Flow

A circuit must have a DRAIN component to act as the output of the circuit. The DRAIN component actively searches for components connected to it and "pulls" the signal through that component and every component coming into it, recursively. Currently, the only way to input a signal to a circuit is via a BUTTON component. 

Here is a simple togglable LED circuit to get you started:
 1. Place one of: DRAIN, LED, BUTTON
 2. Press and drag shift+left click to place wires from source to destination. Wire BUTTON -> LED and LED -> DRAIN
 3. Right-click on the button to toggle the LED 