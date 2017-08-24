As can readily be seen from the following code, the class ****HexCoords**** completely hides the conversion between Rectangular (or User) coordinates and the Canon(ical) coordinate system with axes at 120 degrees. One simply creates an instance of HexCoords in the frame-of-reference available, and allows the data-type to supply either as needed. 

Lazy initialization of the frame of reference **not** supplied on construction trades a small amount of space (for two Boolean flags) for avoidance of conversions when one is not moving to a new reference frame.

Let's look at this code fragment from **HeCoords.cs**:

{{
/// <summary>Create a new instance located at the specified x and y offsets 
/// as interpreted in the Canon(ical) frame.</summary>
public static HexCoords NewCanonCoords (int x, int y) { 
  return new HexCoords(true,  x, y); 
}

/// <summary>Create a new instance located at the specified x and y offsets 
/// as interpreted in the ectangular (User) frame.</summary>
public static HexCoords NewUserCoords  (int x, int y) { 
  return new HexCoords(false, x, y); 
}

static readonly IntMatrix2D MatrixUserToCanon = new IntMatrix2D(2, 1,  0,2,  0,0,  2);
static readonly IntMatrix2D MatrixCanonToUser = new IntMatrix2D(2,-1,  0,2,  0,1,  2);

#region Constructors
private HexCoords(bool isCanon, int x, int y) : this(isCanon, new IntVector2D(x,y)) {}
private HexCoords(bool isCanon, IntVector2D vector) : this() {
  if (isCanon) { Canon = vector; userHasValue  = false; }
  else         { User  = vector; canonHasValue = false; }
}
#endregion

#region Properties
/// <summary>Returns an <c>IntVector2D</c> representing the Canonical (obtuse) 
/// coordinates of this hex.</summary>
 public  IntVector2D Canon {
  get { return canonHasValue ? _Canon : ( Canon = _User * MatrixUserToCanon); }
  set { _Canon = value; canonHasValue = true; userHasValue = false; }
} private IntVector2D _Canon;
bool canonHasValue;

/// <summary>Returns an <c>IntVector2D</c> representing the User (rectangular) 
/// coordinates of this hex.</summary>
 public  IntVector2D User  {
  get { return userHasValue ? _User : ( User = _Canon * MatrixCanonToUser); }
  set { _User = value;  userHasValue = true; canonHasValue = false; }
} private IntVector2D _User;
bool userHasValue;
#endregion
}}

Similarly this fragment from CustomCoords.cs illustrates how the simple definition of two matrices (in the case of transposition or reflection, the same matrix twice) adds another frame of reference possibility.
{{
public static class CustomCoords {

  /// <summary>Return the coordinate vector of this hex in the Custom frame.</summary>
  public static IntVector2D UserToCustom(this HexCoords @this) {
    return @this.User * MatrixUserToCustom;
  }
  /// <summary>Return the coordinate vector of this hex in the User frame.</summary>
  public static HexCoords CustomToUser(this IntVector2D @this) {
    return HexCoords.NewUserCoords(@this * MatrixUserToCustom);
  }

  /// <summary>Initialize the conversion matrices for the Custom coordinate
  /// frame.</summary>
  public static void SetMatrices(IntMatrix2D matrix) { SetMatrices(matrix,matrix); }

  /// <summary>Initialize the conversion matrices for the Custom coordinate
  /// frame.</summary>
  public static void SetMatrices(IntMatrix2D userToCustom, IntMatrix2D customToUser) {
    MatrixUserToCustom = userToCustom;
    MatrixCustomToUser = customToUser;
  }

  /// <summary>Gets the conversion matrix from Custom to Rectangular (User) 
  /// coordinates.</summary>
  public static IntMatrix2D MatrixCustomToUser { get; private set; }

  /// <summary>Gets the conversion matrix from Rectangular (User) to Custom
 /// coordinates.</summary>
  public static IntMatrix2D MatrixUserToCustom { get; private set; }
}
}}