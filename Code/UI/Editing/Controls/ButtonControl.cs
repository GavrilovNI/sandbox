namespace Sandbox.UI.Editing.Controls;

[CustomEditor( ForMethod = true )]
public partial class ButtonControl : BaseControl
{
	public ButtonControl()
	{
	}

	public override void Rebuild()
	{
		DeleteChildren( true );
		if ( Property is null || !Property.IsMethod )
			return;

		var name = Property.TryGetAttribute<ButtonAttribute>( out var attr ) ? (string.IsNullOrWhiteSpace( attr.Title ) ? Property.Name : attr.Title) : Property.Name;

		var button = AddChild( new Button( name, Property.Invoke ) );
	}
}
