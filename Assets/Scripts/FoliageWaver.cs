using UnityEngine;

[RequireComponent( typeof( SpriteRenderer ) )]
[RequireComponent( typeof( BoxCollider2D ) )]
public class FoliageWaver : MonoBehaviour
{
	#region InspectorFields
	[Header( "Wave Properties" )]
	[MinMaxSlider( 0, 1 )]
	public Vector2 waveDissipation = new Vector2( 0.54f, 0.57f );

	[MinMaxSlider( 0, 1 )]
	public Vector2 waveFriction = new Vector2( 0.35f, 0.4f );

	[MinMaxSlider( 0f, 2f )]
	public Vector2 shakeBending = new Vector2( 1.1f, 1.15f );

	[MinMaxSlider( 0f, 4f )]
	public Vector2 shakeSpeed = new Vector2( 3.3f, 3.8f );

	[MinMaxSlider( 0f, 1f )]
	public Vector2 coordinateApplication = new Vector2( 0.02f, 1f );

	[Header( "Rotation Properties" )]
	public Vector2 rotationPivot = new Vector2( 0f, 0f );

	[MinMaxSlider( 0f, 1f )]
	public Vector2 rotationStrength = new Vector2( 0.038f, 0.21f );

	[Range( 0f, 1f )]
	public float rotationLimits = 0.04f;

	[Range( 1f, 6f )]
	public float rotationWaveSpeed = 0.2f;

	[Range( 0f, 5f )]
	public float rotationBounciness = 0.5f;

	[Header( "Others" )]
	public bool underWater = false;
	#endregion InspectorFields

	#region PrivateFields
	private float _wave_scew = 0f;
	private float _wave_force = 0f;

	private Renderer _rend;

	private float _wave_dissipation;
	private float _wave_friction;
	private float _shake_bending;
	private float _shake_speed;
	private float _rotation_strength;

	private float _wave_dissipation_rn = 0.5f;
	private float _wave_friction_rn = 0.5f;
	private float _shake_bending_rn = 0.5f;
	private float _shake_speed_rn = 0.5f;
	private float _rotation_strength_rn = 0.5f;

	private float _time;
	private float _rot_amount;
	private Vector2 _real_pivot;

	//shader property ids
	private int _foliage_shake_prop;

	private int _foliage_zone_application_and_time;
	private int _foliage_rotation_prop;
	private int _foliage_transform_rotation_prop;
	#endregion PrivateFields

	#region Unity
	private void Awake( )
	{
		_real_pivot = rotationPivot;

		//Using ID's to send properties to the shader
		_foliage_shake_prop = Shader.PropertyToID( "_FoliageShake" );
		_foliage_zone_application_and_time = Shader.PropertyToID( "_FoliageZoneApplicationAndTime" );
		_foliage_rotation_prop = Shader.PropertyToID( "_FoliageRotation" );
		_foliage_transform_rotation_prop = Shader.PropertyToID( "_FoliageTransformRotation" );

		_rend = GetComponent<Renderer>( );

		//Makes a calculation of the numbers given min and max of the range in inspector
		_CalculateRandomNumbers( );
		_RecalculateValues( );
	}

	private void FixedUpdate( )
	{
		//Taking time
		_time += Time.fixedDeltaTime;

		//get velocity of the wind that will move the plant and add it to the total force
		if( WeatherData.Instance && WeatherData.Instance.isActiveAndEnabled && WeatherData.Instance.windProperties != null && WeatherData.Instance.activateWind )
		{
			_wave_force += WeatherData.Instance.windProperties.randomVelocity.x * 0.1f;
		}

		//The skew accumulates the force applied over time. 
		_wave_scew += _wave_force;

		//Friction and dissipation makes the plant go back to initial position little by little.
		//The more friction, the more difficult to make the plant move with the force
		float friction_application = ( 1 - _wave_friction );
		_wave_scew *= friction_application;

		//The more dissipation, the quickest the plant goes back to its initial position
		float dissipation_application = ( 1 - ( 0.1f * _wave_dissipation ) );
		_wave_force *= dissipation_application;

		//Rotation of the plant arount the pivot
		int direction = _wave_scew > 0 ? 1 : -1;
		float rot_amount = _CalculateRotationAmount( friction_application );
		Vector3 transform_rotation = Mathf.Deg2Rad * transform.rotation.eulerAngles;

		//Calculations for material before applying to shader
		_real_pivot = new Vector2( rotationPivot.x * direction, rotationPivot.y );
		Vector2 rotation_pivot = ( Vector2 )transform.position + ( _real_pivot );
		float time_for_shader = _time / 20f;
		Vector4 shake = new Vector4( underWater ? 1 : 0, _wave_scew, _shake_bending, _shake_speed );
		Vector3 zone_application_and_time = new Vector3( coordinateApplication.x, coordinateApplication.y, time_for_shader );
		Vector3 rotation = new Vector3( rotation_pivot.x, rotation_pivot.y, rot_amount );

		foreach( Material material in _rend.materials )
		{
			material.SetVector( _foliage_shake_prop, shake );
			material.SetVector( _foliage_zone_application_and_time, zone_application_and_time );
			material.SetVector( _foliage_rotation_prop, rotation );
			material.SetVector( _foliage_transform_rotation_prop, transform_rotation );
		}

		//Recalculates in editor to be able to adjust the waver in real time
#if UNITY_EDITOR
		_RecalculateValues( );
#endif
	}
	#endregion Unity

	#region LocalMethods
	private float _PickRandomBetween( Vector2 min_max_, float random_seed_ )
	{
		//Typical percentage calculation
		return min_max_.x + random_seed_ * ( min_max_.y - min_max_.x );
	}

	private void _RecalculateValues( )
	{
		//RePicks the value with the same seed - same minmax will return same value until the game restarts
		_wave_friction = _PickRandomBetween( waveFriction, _wave_friction_rn );
		_wave_dissipation = _PickRandomBetween( waveDissipation, _wave_dissipation_rn );
		_shake_bending = _PickRandomBetween( shakeBending, _shake_bending_rn );
		_shake_speed = _PickRandomBetween( shakeSpeed, _shake_speed_rn );
		_rotation_strength = _PickRandomBetween( rotationStrength, _rotation_strength_rn );
	}

	private void _CalculateRandomNumbers( )
	{
		//Stores random values to take a number between min and max for the properties
		_wave_dissipation_rn = Random.value;
		_wave_friction_rn = Random.value;
		_shake_bending_rn = Random.value;
		_shake_speed_rn = Random.value;
		_rotation_strength_rn = Random.value;
	}

	private float _CalculateRotationAmount( float friction_ )
	{
		//Calculates how much the plant should rotate depending on the force, bounciness and speed.
		_rot_amount += _wave_force * _rotation_strength * 0.05f;
		_rot_amount += ( 0.005f * rotationBounciness * Mathf.Sin( _wave_force * rotationWaveSpeed ) + _wave_force * 0.005f );

		//Makes friction affect
		_rot_amount *= friction_;

		//Don't rotate further than limits
		_rot_amount = Mathf.Clamp( _rot_amount, -rotationLimits, rotationLimits );

		return _rot_amount;
	}
	#endregion LocalMethods
}