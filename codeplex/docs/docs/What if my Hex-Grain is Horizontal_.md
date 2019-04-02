Easy! Define your map transposed, with the hex-grain vertical, and set the **IsTransposed** flag in **HexgridPanel**. Whenever a mouse-point is supplied by the user, execute the code shown here to transpose it into the map frame of reference as shown here:
{{
protected override void SomeMouse_Event(object sender, MouseEventArgs e) {
  var location = TransposePoint(e.Location);
  // etc.
}
}}
Searching on the string _IsTransposed_ in the example code shows how easy it is. The **Transpose** button in the sample demonstrates this.

Note that transposing a map moves the provided Custom Coordinates' origin to the upper-right from the lower-left. This can be compensated for by replacing the default Custom Coordinates conversion matrix with its transpose.