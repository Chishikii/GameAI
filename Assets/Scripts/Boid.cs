using UnityEngine;

public class Boid : MonoBehaviour
{
    public BoidSettings settings;

    //State
    public Vector3 position;
    public Vector3 forward;
    public Vector3 velocity;

    //To Change
    public Vector3 steeringForce;
    [HideInInspector]
    public Vector3 avgFlockHeading;
    [HideInInspector]
    public Vector3 avgAvoidanceHeading;
    [HideInInspector]
    public Vector3 centreOfFlockmates;
    [HideInInspector]
    public int numPerceivedFlockmates;

    [HideInInspector]
    public Transform pathTarget;
    [HideInInspector]
    public int pathIndex;

    //Cached
    Material material;
    Transform cachedTransform;
    Transform target;

    public enum Steering_Behaviour
    {
        DynamicArrive,
        DynamicFlee,
        DynamicSeek,
        DynamicWander,
        Flocking,
        PathGrapplingHooks
    }
    void Awake()
    {
        material = transform.GetComponentInChildren<MeshRenderer>().material;
        cachedTransform = transform;
    }

    public void Initialize(BoidSettings settings, Transform target)
    {
        this.target = target;
        this.pathTarget = null;
        pathIndex = 0;
        this.settings = settings;
        position = cachedTransform.position;
        forward = cachedTransform.forward;

        float startSpeed = settings.maxSpeed / 2;
        velocity = transform.forward * startSpeed;
        avgAvoidanceHeading = Vector3.zero;
        avgFlockHeading = Vector3.zero;
    }

    public void UpdateBoid(Steering_Behaviour behaviourType, bool targeting)
    {
        switch (behaviourType)
        {
            case Steering_Behaviour.DynamicArrive:
                DynamicArrive();
                break;
            case Steering_Behaviour.DynamicFlee:
                DynamicFlee();
                break;
            case Steering_Behaviour.DynamicSeek:
                DynamicSeek();
                break;
            case Steering_Behaviour.DynamicWander:
                DynamicWander();
                break;
            case Steering_Behaviour.Flocking:
                Flocking(targeting);
                break;
            case Steering_Behaviour.PathGrapplingHooks:
                PathGrapplingHooks();
                break;
        }

        UpdateState();
    }

    public void SetColour(Color color)
    {
        if (material != null)
        {
            material.color = color;
        }
    }

    void UpdateState()
    {
        //collison
        if (IsHeadingForCollision())
        {
            Vector3 collisionAvoidDir = ObstacleRays();
            Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * settings.avoidCollisionWeight;
            steeringForce += collisionAvoidForce;
        }

        //velocity
        steeringForce = Vector3.ClampMagnitude(steeringForce, settings.maxSteerForce);
        velocity = Vector3.ClampMagnitude(velocity, settings.maxSpeed);
        velocity += steeringForce * Time.deltaTime;

        //position and direction
        position += velocity * Time.deltaTime;
        forward = velocity.normalized;
        transform.position = position;
        transform.forward = forward;

        Debug.DrawLine(position, position + velocity, Color.red);
        Debug.DrawLine(position, position + steeringForce, Color.green);
    }

    void DynamicSeek()
    {
        Vector3 offsetToTarget = (target.position - position);
        steeringForce = offsetToTarget.normalized * settings.maxSteerForce;
    }

    void PathGrapplingHooks()
    {
        Vector3 offsetToTarget = (pathTarget.position - position);
        steeringForce = offsetToTarget.normalized * settings.maxSteerForce;
    }

    void DynamicFlee()
    {
        //Max Distance?
        Vector3 offsetToTarget = (position - target.position);
        steeringForce = offsetToTarget.normalized * settings.maxSteerForce;
    }

    void DynamicWander()
    {
        if (Random.value < 0.001)
        {
            Vector3 circleCenter = forward * settings.wanderCircleOffset;
            Vector3 randomPoint = Random.insideUnitCircle;

            //needs random vector with angle x max 10
            Vector3 displacement = new Vector3(randomPoint.x, randomPoint.y) * settings.wanderCircleRadius;
            displacement = Quaternion.LookRotation(velocity) * displacement;

            steeringForce = circleCenter + displacement;
        }
    }

    void DynamicArrive()
    {
        Vector3 desiredVelocity = (target.position - position);
        float distance = desiredVelocity.magnitude;

        if (distance > settings.arriveSlowRadius)
        {
            desiredVelocity = desiredVelocity.normalized * settings.maxSpeed;
        }
        else
        {
            desiredVelocity = desiredVelocity.normalized * settings.maxSpeed * (distance / settings.arriveSlowRadius);
        }

        steeringForce = desiredVelocity - velocity;

        Debug.DrawLine(position, position + steeringForce, Color.magenta);
        Debug.DrawLine(position, target.position, Color.green);
        Debug.DrawLine(position, position + velocity, Color.red);
    }

    void Flocking(bool targeting)
    {
        steeringForce = Vector3.zero;
        if (target != null && targeting)
        {
            Vector3 offsetToTarget = (target.position - position);
            steeringForce = SteerTowards(offsetToTarget) * settings.targetWeight;
        }

        if (numPerceivedFlockmates > 1)
        {
            centreOfFlockmates /= numPerceivedFlockmates;

            Vector3 offsetToFlockmatesCentre = (centreOfFlockmates - position);

            var alignmentForce = SteerTowards(avgFlockHeading) * settings.alignWeight;
            var cohesionForce = SteerTowards(offsetToFlockmatesCentre) * settings.cohesionWeight;
            var seperationForce = SteerTowards(avgAvoidanceHeading) * settings.seperateWeight;

            steeringForce += alignmentForce;
            steeringForce += cohesionForce;
            steeringForce += seperationForce;
        }
        UpdateState();
    }

    bool IsHeadingForCollision()
    {
        RaycastHit hit;
        if (Physics.SphereCast(position, settings.boundsRadius, forward, out hit, settings.collisionAvoidDst, settings.obstacleMask))
        {
            return true;
        }
        return false;
    }

    Vector3 ObstacleRays()
    {
        Vector3[] rayDirections = BoidHelper.directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 dir = transform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(position, dir);
            if (!Physics.SphereCast(ray, settings.boundsRadius, settings.collisionAvoidDst, settings.obstacleMask))
            {
                return dir;
            }
        }

        return forward;
    }

    Vector3 SteerTowards(Vector3 vector)
    {
        Vector3 v = vector.normalized * settings.maxSpeed - velocity;
        return Vector3.ClampMagnitude(v, settings.maxSteerForce);
    }

    float RandomGaussian()
    {
        float rand1 = Random.Range(0.0f, 1.0f);
        float rand2 = Random.Range(0.0f, 1.0f);
        float rand3 = Random.Range(0.0f, 1.0f);

        return (rand1 + rand2 + rand3) / 3f;
    }
}