﻿using UnityEngine;
using System.Runtime.InteropServices;
using System;

class CPUCapabilityDetector : MonoBehaviour
{
    enum SYSTEM_LEVELS
    {
        LOW_END_SYSTEM,
        HIGH_END_SYSTEM,
        NUM_SYSTEMS
    };

    #region Essentials

    // These functions are used to allocate and initialize resources and to deallocate, respectively
    [DllImport("CapabilityTester")]
    private static extern void InitializeResources();
    [DllImport("CapabilityTester")]
    private static extern void FreeResources();
    // This is used to determine if running on an Intel CPU
    [DllImport("CapabilityTester")]
    private static extern bool IsIntelCPU();

    // These are the getters used in the bucketing decision
    // They can be used to create your own bucketing solution
    [DllImport("CapabilityTester")]
    private static extern int GetNumLogicalCores();
    [DllImport("CapabilityTester")]
    private static extern double GetUsablePhysMemoryGB();
    [DllImport("CapabilityTester")]
    private static extern double GetMaxBaseFrequency();
    [DllImport("CapabilityTester")]
    private static extern double GetCacheSizeMB();
    [DllImport("CapabilityTester")]
    private static extern SYSTEM_LEVELS CategorizeSystemCPU();
    #endregion

    #region Threshold Getters and Setters
    // These are the setters for the thresholds used to compare against in the bucketing decision
    // Default values reflect stats of the i7-6700k CPU as shown @ ark.intel.com
    // int LogicalCoreThreshold = 8;			// # of Threads @ ark.intel.com
    // double MemoryThresholdGB = 7.7;
    // double MaxFrequencyThresholdMhz = 4000;	// Processor Base Frequency @ ark.intel.com
    // double FrequencyMarginOfError = 50.0;
    // double CacheSizeThresholdMB = 8;			// Cache @ ark.intel.com
    // double FrequencyBiasMhz = 50;			// Bias used in bucketing calculation
    [DllImport("CapabilityTester")]
    private static extern void SetThresholdLogicalCoreCount(int count);
    [DllImport("CapabilityTester")]
    private static extern void SetThresholdMemoryGB(double memoryThreshold);
    [DllImport("CapabilityTester")]
    private static extern void SetThresholdMaxBaseFrequencyMhz(double freqMaxBaseMhz);
    [DllImport("CapabilityTester")]
    private static extern void SetThresholdCacheSizeMB(double cacheSize);


    // Getters to check current values of the thresholds
    [DllImport("CapabilityTester")]
    private static extern int GetThresholdLogicalCoreCount();
    [DllImport("CapabilityTester")]
    private static extern double GetThresholdMemoryGB();
    [DllImport("CapabilityTester")]
    private static extern double GetThresholdMaxBaseFrequencyMhz();
    [DllImport("CapabilityTester")]
    private static extern double GetThresholdCacheSizeMB();
    [DllImport("CapabilityTester")]
    private static extern double GetMaxBaseFrequencyBias();
    #endregion

    #region Extras

    // Not used in the internal calculation, but are exposed to help
    // create more elaborate bucketing systems
    [DllImport("CapabilityTester")]
    private static extern int GetNumPhysicalCores();
    [DllImport("CapabilityTester")]
    private static extern double GetComittedMemoryMB();
    [DllImport("CapabilityTester")]
    private static extern double GetAvailableMemoryMB();

    // These are used in GetMaxFrequency() calculation.
    // GetCoreFreq returns the frequency at collection time, while PercMaxFrequency returns the percentage of the maximum frequency.
    [DllImport("CapabilityTester")]
    private static extern double GetCoreFreq();
    [DllImport("CapabilityTester")]
    private static extern double GetCorePercMaxFrequency();
    #endregion

    void Awake()
    {
        if(IsIntelCPU())
        {
            Debug.Log("You are running on an Intel CPU");
            InitializeResources();
            Debug.Log("Default CPU threshold values based on i7-6700k specs @ ark.intel.com");
            Debug.Log("Default core count threshold: " + GetThresholdLogicalCoreCount());
            Debug.Log("Default maximum base frequency threshold: " + GetThresholdMaxBaseFrequencyMhz());
            Debug.Log("Default cache size threshold: " + GetThresholdCacheSizeMB());
            Debug.Log("Default memory threshold: " + GetThresholdMemoryGB());
            Debug.Log("");

            Debug.Log("The following are values queried from the system:");
            Debug.Log("Number of logical cores = " + GetNumLogicalCores());
            Debug.Log("Total physical memory = " + GetUsablePhysMemoryGB());
            Debug.Log("Maximum base frequency per core = " + GetMaxBaseFrequency());
            Debug.Log("Cache size = " + GetCacheSizeMB());

            SYSTEM_LEVELS systemLevel = CategorizeSystemCPU();
            if (systemLevel == SYSTEM_LEVELS.HIGH_END_SYSTEM)
            {
                Debug.Log("This system has been categorized as high end.  System values exceeded threshold.");
            }
            else
            {
                Debug.Log("This system has been categorized as low end.  System values didn't exceed threshold.");
            }
            Debug.Log("");

            int newCoreCountThreshold = 4;
            double newMemoryThresholdGB = 3.7;
            double newMaxBaseFrequencyThresholdMhz = 2000;
            double newCacheSizeThreshold = 4;
            SetThresholdLogicalCoreCount(newCoreCountThreshold);
            SetThresholdMemoryGB(newMemoryThresholdGB);
            SetThresholdMaxBaseFrequencyMhz(newMaxBaseFrequencyThresholdMhz);
            SetThresholdCacheSizeMB(newCacheSizeThreshold);
            Debug.Log("Setting new core count threshold to: " + GetThresholdLogicalCoreCount());
            Debug.Log("Setting new memory threshold to: " + GetThresholdMemoryGB());
            Debug.Log("Setting new maximum base frequency threshold to: " + GetThresholdMaxBaseFrequencyMhz());
            Debug.Log("Setting new cache size threshold to: " + GetThresholdCacheSizeMB());

            systemLevel = CategorizeSystemCPU();
            if (systemLevel == SYSTEM_LEVELS.HIGH_END_SYSTEM)
            {
                Debug.Log("With the new thresholds, this system has been categorized as high end.");
            }
            else
            {
                Debug.Log("With the new thresholds, this system has been categorized as low end.");
            }
            Debug.Log("");

            Debug.Log("Extras:");
            Debug.Log("Comitted Memory (MB) = " + GetComittedMemoryMB());
            Debug.Log("Available Memory (MB) = " + GetAvailableMemoryMB());
            Debug.Log("Num physical cores = " + GetNumPhysicalCores());
        }
        else
        {
            Debug.Log("You are not running on an Intel CPU");
        }

    }

    void Update()
    {
    }
}