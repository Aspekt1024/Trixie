﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore
{
    public enum Layers
    {
        Projectile, Enemy, Player, Terrain, Shield
    }

    public class TrixieLayers : MonoBehaviour
    {
        private static TrixieLayers instance;
        private Dictionary<Layers, LayerMask> layerMasks;

        public static LayerMask GetMask(Layers layer)
        {
            return instance.layerMasks[layer];
        }

        private void Start()
        {
            if (instance == null)
            {
                instance = this;
                GenerateLayerDictionary();
            }
            else
            {
                Destroy(this);
            }
        }

        private void GenerateLayerDictionary()
        {
            layerMasks = new Dictionary<Layers, LayerMask>();
            foreach (Layers layer in Enum.GetValues(typeof(Layers)))
            {
                layerMasks.Add(layer, LayerMask.NameToLayer(layer.ToString()));
            }
        }
    }
}
