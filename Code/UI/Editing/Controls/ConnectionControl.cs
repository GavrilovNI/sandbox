namespace Sandbox.UI.Editing.Controls;

[CustomEditor( typeof( Connection ) )]
public class ConnectionControl : BaseControl
{
	public override bool SupportsMultiEdit => true;

	private DropDown? _dropDown;

	public ConnectionControl()
	{

	}


	public override void Rebuild()
	{
		if ( Property == null )
		{
			DeleteChildren();
			return;
		}

		CreateDropdown();
	}

	private void CreateDropdown()
	{
		_dropDown = AddChild( new DropDown() );

		_dropDown.Options.Add( new Option( "None", string.Empty, null ) );
		foreach ( var connection in Connection.All )
		{
			_dropDown.Options.Add( new Option( connection.DisplayName, string.Empty, connection ) );
		}

		_dropDown.Value = Property.GetValue<object>();
		_dropDown.ValueChanged = SetValue;
		UpdateText();
	}

	private void SetValue( string value )
	{
		Connection connection;
		if ( value is null || value == string.Empty )
		{
			connection = null;
		}
		else
		{
			connection = Connection.All.FirstOrDefault( x => x.ToString() == value );
			if ( connection is null )
				return;
		}

		Property.SetValue( connection );
		UpdateText();
	}

	private void UpdateText()
	{
		_dropDown?.Text = Property.GetValue<Connection>()?.DisplayName ?? "None";
	}
}
