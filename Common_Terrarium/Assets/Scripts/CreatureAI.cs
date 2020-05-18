using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

namespace Assets.Scripts
{
    /// <summary>
    /// This is the class where most of the work will happen,
    /// like in the previous assignments.
    /// </summary>
    public class CreatureAI : Agent
    {
        private Creature creature;
        private int n_regions = 6;

        public void Start()
        {
            Debug.Log($"Creature AI is ready");
            creature = GetComponent<Creature>();
        }


        public override void OnEpisodeBegin()
        {

        }

        public float[] GetSurroundingInfo()
        {
            float[] surroundings = new float[n_regions];
            float angle_offset = 360f / (2f * n_regions); // this is so forward region is not split
            List<GameObject> close_plants = creature.Sensor.SensePlants(creature);
            foreach (GameObject plant in close_plants)
            {
                Vector3 relative_pos = plant.transform.position - this.transform.position;
                float angle = Vector3.SignedAngle(this.transform.forward, relative_pos) - angle_offset;

                int region = (n_regions / 2) + (int)(angle * n_regions / 360f);
                surroundings[region] = 1;
            }
            return surroundings;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(creature.Energy);
            float[] surroning_info = GetSurroundingInfo();
            foreach (float value in surroning_info)
            {
                sensor.AddObservation(value);
            }

            // sensor.AddObservation(creature.Size);
            // sensor.AddObservation(creature.MaxSpeed);
            // sensor.AddObservation(creature.initialSensingRadius);
        }

        public override void OnActionReceived(float[] vectorAction)
        {
            int x_dir = vectorAction[0];  // X direction     (-1, 0, 1)
            int z_dir = vectorAction[1];  // Z direction     (-1, 0, 1)
            int speed = vectorAction[2];  // Movement speed  (1, 2, 3)  # slow medium fast
            int reproduce = vectorAction[3];  // Reproduce   (0, 1)  No, Yes
            float movement_speed = (((float)speed) / 3f);
            Vector3 direction = new Vector3(x_dir, 0, z_dir);
            creature.Move(direction, movement_speed);
            if (reproduce == 1)
            {
                creature.Reproduce();
            }
        }

        public override void Heuristic(float[] actionsOut)
        {
            actionsOut[0] = Input.GetAxis("Horizontal");
            actionsOut[1] = Input.GetAxis("Vertical");
        }
        // public void Update()
        // {
        //     //here, you can call creature.Move
        //     // you can make it sense the surroundings and reproduce, mutation is encompassed in the IReproduction implementation

        //     /*creature.Move(...)
        //     *creature.Sensor.SensePreys()
        //     *creature.Reproduce()
        //     */

        //     //Current example :
        //     var food = creature.Sensor.SensePlants(creature);
        //     Vector3 closestFood = Vector3.zero;
        //     float bestDistance = Vector3.Distance(closestFood, transform.position);
        //     foreach(var foodPiece in food)
        //     {
        //         if (Vector3.Distance(foodPiece.transform.position, transform.position) < bestDistance)
        //         {
        //             bestDistance = Vector3.Distance(foodPiece.transform.position, transform.position);
        //             closestFood = foodPiece.transform.position;
        //         }
        //     }
        //     if (closestFood != Vector3.zero)
        //     {
        //         Debug.DrawLine(transform.position, closestFood, Color.red);
        //         creature.Move(closestFood - transform.position, 1f);
        //     }

        // }

        /// This function is called when your creature is within an acceptable
        /// range to eat some food, adapted to your regime of course.
        public virtual void OnAccessibleFood(GameObject food)  // For now lets always eat
        {
            creature.Eat(food);
            if (creature.Energy > 0.2 * creature.MaxEnergy && UnityEngine.Random.Range(0, 1) < 0.1f) creature.Reproduce();
        }
    }
}
