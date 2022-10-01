﻿using NPCs.Navigation;
using UnityEngine;

namespace Misc
{
    public class ServiceLocator : MonoBehaviour
    {
        private static ServiceLocator _instance;
        
        [SerializeField] private Camera _camera;
        [SerializeField] private WaypointsContainer _waypointsContainer;
        [SerializeField] private GameObject _parasitePrefab;

        public static Camera Camera => Instance._camera;
        public static WaypointsContainer WaypointsContainer => Instance._waypointsContainer;
        public static GameObject ParasitePrefab => Instance._parasitePrefab;

        public static ServiceLocator Instance
        {
            get
            {
                if (_instance is null)
                {
                    return FindObjectOfType<ServiceLocator>();
                }
                return _instance;
            }
        }
    }
}