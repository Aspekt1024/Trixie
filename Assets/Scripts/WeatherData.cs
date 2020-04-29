using System;
using UnityEngine;

public class WindProperties
{
	#region Actions
	public event Action OnPropertiesChanged;
	#endregion Actions

	#region PrivateMethods
	private float _time_passed;

	private Vector2 _frequency;
	private Vector2 _min_velocity;
	private Vector2 _max_velocity;

	private float _random_frequency;
	private Vector2 _random_velocity;

	private float _ran_for_frequency;
	private Vector2 _ran_for_velocity;
	#endregion PrivateMethods

	#region Accessors
	public Vector2 randomVelocity
	{
		get
		{
			return _random_velocity;
		}
	}
	#endregion Accessors

	#region Constructor
	public WindProperties( Vector2 frequency_, Vector2 min_velocity_, Vector2 max_velocity_ )
	{
		ResetProperties( frequency_, min_velocity_, max_velocity_ );
	}

	#endregion Constructor

	#region PublicMethods
	public void ResetProperties( Vector2 frequency_, Vector2 min_velocity_, Vector2 max_velocity_ )
	{
		//If something changed, assign it again and make recalculations
		bool something_changed = frequency_ != _frequency || min_velocity_ != _min_velocity || max_velocity_ != _max_velocity;

		if( something_changed )
		{
			_frequency = frequency_;
			_min_velocity = min_velocity_;
			_max_velocity = max_velocity_;

			_CalculateRandomNumbers( );
			_RecalculatePropertyValues( );
		}
	}

	public void Update( )
	{
		_time_passed += Time.deltaTime;

		//If more time than the frequency set passed, we recalculate numbers again to make wind organic and change with time (frequency)
		if( ( _frequency.x > 0f || _frequency.y > 0f ) && _time_passed >= _random_frequency )
		{
			_CalculateRandomNumbers( );
			_RecalculatePropertyValues( );

			_time_passed = 0f;

			if( OnPropertiesChanged != null )
			{
				OnPropertiesChanged( );
			}
		}
	}

	#endregion PublicMethods

	#region PrivateMethods
	private void _CalculateRandomNumbers( )
	{
		_ran_for_frequency = UnityEngine.Random.value;
		_ran_for_velocity = new Vector2( UnityEngine.Random.value, UnityEngine.Random.value );
	}

	private float _PickRandomBetween( Vector2 min_max_, float precreated_random_ )
	{
		return min_max_.x + precreated_random_ * ( min_max_.y - min_max_.x );
	}

	private Vector2 _PickRandomBetweenVectors( Vector2 min_, Vector2 max_, Vector2 precreated_randoms_ )
	{
		float x = min_.x + precreated_randoms_.x * ( max_.x - min_.x );
		float y = min_.y + precreated_randoms_.y * ( max_.y - min_.y );

		return new Vector2( x, y );
	}

	private void _RecalculatePropertyValues( )
	{
		_random_velocity = _PickRandomBetweenVectors( _min_velocity, _max_velocity, _ran_for_velocity );
		_random_frequency = _PickRandomBetween( _frequency, _ran_for_frequency );
	}

	#endregion PrivateMethods
}

public class WeatherData : Singleton<WeatherData>
{
	#region Actions
	public event Action OnChange;

	#endregion Actions

	#region InspectorField
	[Header( "Wind" )]
	public bool activateWind;

	[MinMaxSlider( 0f, 10f )]
	public Vector2 minMaxFrequency;

	public Vector2 minVelocity;
	public Vector2 maxVelocity;
	#endregion InspectorField

	#region Accessors
	public WindProperties windProperties
	{
		get
		{
			return _wind;
		}
	}
	#endregion Accessors

	#region privateFields
	private WindProperties _wind;

	//previous
	private Vector2 _previous_min_velocity;
	private Vector2 _previous_max_velocity;
	private Vector2 _previous_min_max_frequency;
	private bool _previous_activate_wind;
	#endregion privateFields

	#region Unity
#if UNITY_EDITOR
	//If we enable or disable the component, we have to update
	private void OnEnable( )
	{
		if( OnChange != null )
		{
			OnChange( );
		}
	}

	private void OnDisable( )
	{
		if( OnChange != null )
		{
			OnChange( );
		}
	}
#endif

	private void Awake( )
	{
		if( activateWind )
		{
			_wind = new WindProperties( minMaxFrequency, minVelocity, maxVelocity );
		}
	}

	private void Update( )
	{
		//Only in Unity Editor, if something changes we have to get those values and pick a random between them again
#if UNITY_EDITOR
		bool something_changed = _previous_activate_wind != activateWind;

		if( activateWind )
		{
			something_changed |= minVelocity != _previous_min_velocity || maxVelocity != _previous_max_velocity || minMaxFrequency != _previous_min_max_frequency;

			if( something_changed )
			{
				_wind = new WindProperties( minMaxFrequency, minVelocity, maxVelocity );
			}

			_wind.Update( );
		}
#endif
	}

	private void LateUpdate( )
	{
		//Set new values as previous
		_previous_min_velocity = minVelocity;
		_previous_max_velocity = maxVelocity;
		_previous_min_max_frequency = minMaxFrequency;
		_previous_activate_wind = activateWind;
	}
	#endregion Unity
}