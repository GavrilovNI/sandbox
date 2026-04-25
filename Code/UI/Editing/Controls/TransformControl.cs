namespace Sandbox.UI.Editing.Controls;

[CustomEditor( typeof( Transform ) )]
public partial class TransformControl : BaseControl
{
	public override bool SupportsMultiEdit => true;

	public TransformControl()
	{
	}

	public override void Rebuild()
	{
		if ( Property == null ) return;

		DeleteChildren( true );

		if ( Property.TryGetAsObject( out var so ) )
		{
			var positionProperty = so.GetProperty( "Position" );
			var rotationProperty = so.GetProperty( "Rotation" );
			var scaleProperty = so.GetProperty( "Scale" );

			AddChild( BaseControl.CreateFor( positionProperty ) );
			AddChild( BaseControl.CreateFor( rotationProperty ) );
			AddChild( BaseControl.CreateFor( scaleProperty ) );
		}
	}
}
