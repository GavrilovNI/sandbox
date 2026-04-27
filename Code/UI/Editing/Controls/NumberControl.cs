namespace Sandbox.UI.Editing.Controls;

[CustomEditor( typeof( int ) )]
[CustomEditor( typeof( long ) )]
[CustomEditor( typeof( decimal ) )]
[CustomEditor( typeof( double ) )]
public class NumberControl : NumberEntry
{
}
