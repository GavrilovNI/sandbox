using Sandbox.UI.Construct;
using System.Reflection;

namespace Sandbox.UI.Editing.Controls;

[CustomEditor( typeof( Enum ) )]
[CustomEditor( typeof( ComponentFlags ) )]
[CustomEditor( typeof( GameObjectFlags ) )]
public class EnumControl : BaseControl
{
	public override bool SupportsMultiEdit => true;

	public EnumControl()
	{

	}

	public override void Rebuild()
	{
		if ( Property == null ) return;
		if ( !Property.PropertyType.IsEnum ) return;

		var options = TypeLibrary.GetEnumDescription( Property.PropertyType );
		if ( options == null )
		{
			Log.Warning( $"Couldn't get enum description for {Property.PropertyType}" );
			return;
		}

		CreateDropdown();
	}

	private void CreateDropdown()
	{
		var dd = AddChild( new EnumDropDown( Property.IsNullable ? Property.NullableType : Property.PropertyType ) );
		dd.Value = Property.GetValue<object>();
		dd.ValueChanged = ( val ) => Property.SetValue( val );
	}

	private class EnumDropDown : PopupButton
	{
		protected IconPanel DropdownIndicator;

		[Parameter]
		public System.Action<Enum> ValueChanged { get; set; }

		[Parameter]
		public System.Func<List<Option>> BuildOptions { get; set; }


		private readonly List<Option> _options = [];

		[Parameter]
		public List<Option> Options
		{
			get => _options;
		}

		bool _flags;
		Type _type;
		Enum[] _values;
		string[] _names;

		Enum _value;

		[Parameter]
		public override object Value
		{
			get => _value;
			set
			{
				if ( _value == value )
					return;

				_value = (Enum)Enum.ToObject( _type, value );
				UpdateText();

				ValueChanged?.Invoke( _value );
			}
		}

		public EnumDropDown( Type type )
		{
			_type = type;
			_values = [.. Enum.GetValues( type ).Cast<Enum>()];
			_names = Enum.GetNames( type );
			_flags = _type.GetCustomAttribute<FlagsAttribute>() is not null;

			AddClass( "dropdown" );
			DropdownIndicator = Add.Icon( "expand_more", "dropdown_indicator" );

			PopulateOptionsFromType( type );
			UpdateText();
		}

		public override void SetPropertyObject( string name, object value )
		{
			base.SetPropertyObject( name, value );
		}

		private void PopulateOptionsFromType( Type type )
		{
			if ( type.IsEnum )
			{
				foreach ( var item in TypeLibrary.GetEnumDescription( type ) )
				{
					Options.Add( new Option( item.Title, item.Icon, item.ObjectValue ) );
				}

				return;
			}
		}

		public override void Open()
		{
			if ( Popup is not null && _flags )
			{
				Popup.Delete();
				Popup = null;
				return;
			}

			Popup = new( this, Popup.PositionMode.BelowStretch, 0.0f )
			{
				CloseWhenParentIsHidden = true,
				StayOpen = _flags,
			};
			Popup.AddClass( "flat-top" );

			foreach ( var option in Options )
			{
				var o = Popup.AddOption( option.Title, option.Icon, () => Select( option ) );
				UpdatePopupStyles();
			}
		}

		private void UpdateText()
		{
			Text = string.Join( ", ", _names.Where( IsSelected ) );
		}

		private void UpdatePopupStyles()
		{
			if ( Popup is null )
				return;

			foreach ( var option in Popup.Children )
			{
				if ( option is Button btn )
				{
					if ( IsSelected( btn.Text ) )
						option.Style.BackgroundColor = Color.Parse( "#00af" );
					else
						option.Style.BackgroundColor = null;
					btn.Style.Dirty();
				}
			}
		}

		public bool IsSelected( object value )
		{
			if ( value is string str )
			{
				var index = _names.IndexOf( str.Replace( " ", string.Empty ), StringComparer.OrdinalIgnoreCase );
				value = _values[index];
			}
			if ( value is not Enum )
			{
				value = Enum.ToObject( _type, value );
			}



			return _flags ? _value.HasFlag( (Enum)value ) : _value.Equals( value );
		}

		private void Select( Option option )
		{
			var value = option.Value;

			if ( IsSelected( value ) )
			{
				if ( _flags )
					Value = Convert.ToInt64( _value ) & (~Convert.ToInt64( value ));
			}
			else
			{
				if ( _flags )
				{
					Value = Convert.ToInt64( _value ) | Convert.ToInt64( value );
				}
				else
				{
					Value = value;
				}

			}

			UpdatePopupStyles();
		}

		protected override void OnParametersSet()
		{
			// Only clear if we have some options to populate
			if ( Children.Any( x => x.ElementName.Equals( "option", StringComparison.OrdinalIgnoreCase ) ) ) Options.Clear();

			foreach ( var child in Children )
			{
				if ( child.ElementName.Equals( "option", StringComparison.OrdinalIgnoreCase ) )
				{
					var o = new Option();
					o.Title = string.Join( "", child.Descendants.OfType<Label>().Select( x => x.Text ) );
					o.Value = child.GetAttribute( "value", o.Title );
					o.Icon = child.GetAttribute( "icon", null );

					Options.Add( o );
				}
			}
		}
	}
}
