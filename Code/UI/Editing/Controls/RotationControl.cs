namespace Sandbox.UI.Editing.Controls;

[CustomEditor( typeof( Rotation ) )]
public partial class RotationControl : BaseControl
{
	public override bool SupportsMultiEdit => true;

	NumberEntry _pitch;
	NumberEntry _yaw;
	NumberEntry _roll;

	public RotationControl()
	{
		_pitch = AddChild<NumberEntry>( "pitch" );
		_yaw = AddChild<NumberEntry>( "yaw" );
		_roll = AddChild<NumberEntry>( "roll" );
	}

	public override void Rebuild()
	{
		if ( Property == null ) return;

		_pitch.Property = TypeLibrary.CreateProperty( "Pitch",
			() => Property.GetValue<Rotation>().Pitch(),
			v => Property.SetValue( Property.GetValue<Rotation>().Angles().WithPitch( v ).ToRotation() ),
			Property.GetAttributes().ToArray(),
			Property.Parent );

		_yaw.Property = TypeLibrary.CreateProperty( "Yaw",
			() => Property.GetValue<Rotation>().Yaw(),
			v => Property.SetValue( Property.GetValue<Rotation>().Angles().WithYaw( v ).ToRotation() ),
			Property.GetAttributes().ToArray(),
			Property.Parent );

		_roll.Property = TypeLibrary.CreateProperty( "Roll",
			() => Property.GetValue<Rotation>().Roll(),
			v => Property.SetValue( Property.GetValue<Rotation>().Angles().WithRoll( v ).ToRotation() ),
			Property.GetAttributes().ToArray(),
			Property.Parent );

		_pitch.Style.Display = DisplayMode.Flex;
		_yaw.Style.Display = DisplayMode.Flex;
		_roll.Style.Display = DisplayMode.Flex;
	}
}
