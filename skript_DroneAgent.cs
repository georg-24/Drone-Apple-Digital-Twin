using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class DroneAgent : Agent
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform targetApple;
    [SerializeField] private float maxForce = 10f;
    private bool nearRedApple = false;
    private GameObject currentApple;

    public override void OnEpisodeBegin()
    {
        // Сброс позиции
        transform.localPosition = new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-5f, 5f));
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        // Поиск нового целевого яблока
        GameObject[] apples = GameObject.FindGameObjectsWithTag("RedApple");
        if (apples.Length > 0)
            targetApple = apples[Random.Range(0, apples.Length)].transform;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition.x / 20f);
        sensor.AddObservation(transform.localPosition.z / 20f);
        sensor.AddObservation(transform.localPosition.y / 10f);
        sensor.AddObservation(rb.velocity.x / 10f);
        sensor.AddObservation(rb.velocity.z / 10f);
        sensor.AddObservation(rb.velocity.y / 8f);
        
        if (targetApple != null)
        {
            Vector3 toTarget = targetApple.position - transform.position;
            sensor.AddObservation(toTarget.x / 20f);
            sensor.AddObservation(toTarget.z / 20f);
            sensor.AddObservation(toTarget.magnitude / 15f);
        }
        else
            sensor.AddObservation(Vector3.zero);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float forward = actions.DiscreteActions[0] - 1f;
        float right = actions.DiscreteActions[1] - 1f;
        float up = actions.DiscreteActions[2] - 1f;
        bool grab = actions.DiscreteActions[3] == 1;
        
        Vector3 force = new Vector3(right * maxForce, up * maxForce * 0.8f, forward * maxForce);
        rb.AddForce(force);
        
        if (grab && nearRedApple && currentApple != null)
        {
            AddReward(5f);
            Destroy(currentApple);
            nearRedApple = false;
            EndEpisode();  // успешное завершение
        }
        
        // Штраф за время
        AddReward(-0.0005f);
        
        // Штраф за падение/вылет
        if (transform.localPosition.y < 0.2f || Mathf.Abs(transform.localPosition.x) > 25f || Mathf.Abs(transform.localPosition.z) > 25f)
        {
            AddReward(-1f);
            EndEpisode();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RedApple"))
        {
            nearRedApple = true;
            currentApple = other.gameObject;
        }
        else if (other.CompareTag("GreenApple"))
        {
            AddReward(-2f);
            Destroy(other.gameObject);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("RedApple") && currentApple == other.gameObject)
            nearRedApple = false;
    }
}
