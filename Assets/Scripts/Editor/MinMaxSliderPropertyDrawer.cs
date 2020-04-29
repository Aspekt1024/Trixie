using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer( typeof( MinMaxSliderAttribute ) )]
class MinMaxSliderPropertyDrawer : PropertyDrawer
{
	private const int _control_height = 16;

	public override float GetPropertyHeight( SerializedProperty property_, GUIContent label_ )
	{
		return base.GetPropertyHeight( property_, label_ ) + _control_height * 2f;
	}

	public override void OnGUI( Rect position_, SerializedProperty property_, GUIContent label_ )
	{
		label_ = EditorGUI.BeginProperty( position_, label_, property_ );

		if( property_.propertyType == SerializedPropertyType.Vector2 )
		{
			Vector2 range = property_.vector2Value;
			MinMaxSliderAttribute attr = attribute as MinMaxSliderAttribute;

			range.x = Mathf.Max( range.x, attr.min );
			range.y = Mathf.Min( range.y, attr.max );

			//EditorGUI.BeginChangeCheck( );
			range = EditorGUI.Vector2Field( position_, label_, range );

			Rect position = EditorGUI.IndentedRect( position_ );
			position.y += _control_height * 1.5f;
			position.height = _control_height + 5;
			EditorGUI.MinMaxSlider( position, ref range.x, ref range.y, attr.min, attr.max );

			//if( EditorGUI.EndChangeCheck( ) )
			{
				property_.vector2Value = range;
			}
		}
		else
		{
			EditorGUI.LabelField( position_, label_, "Use only with Vector2" );
		}

		EditorGUI.EndProperty( );
	}
}