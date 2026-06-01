#region Copyright -------------------------------------------------------
// Copyright © 2009, ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Author(s)  : Rolf Waterman, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/Optimalisering.Aimms/Input/Partition.cs 3     31-03-09 10:30 Waterman $
// $NoKeywords: $
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Aimms;

namespace OptimaliseRing.Aimms
{
  /// <summary>
  /// Partition settings
  /// </summary>
  [Serializable]
  public class Partition
  {
    #region Instance Variables -----------------------------------------------

    /// <summary>
    /// Partitionnaam
    /// </summary>
    private string m_partition;
    /// <summary>
    /// Id gekregen in Aimms
    /// </summary>
    private int m_partitionCardId;
    /// <summary>
    /// Partition_start_moment
    /// eenheid: year
    /// </summary>
    private int m_partition_start_moment;
    /// <summary>
    /// Partition_end_moment
    /// eenheid: year
    /// </summary>
    private int m_partition_end_moment;
    /// <summary>
    /// Partition_no_periods
    /// eenheid: -
    /// </summary>
    private int m_partition_no_periods;
    /// <summary>
    /// Partition_pct
    /// eenheid: %
    /// </summary>
    private double m_partition_pct;

    #endregion Instance Variables --------------------------------------------

    #region Properties -------------------------------------------------------

    /// <summary>
    /// Gets or sets the partition.
    /// </summary>
    /// <value>The partition.</value>
    public string PartitionName
    {
      get { return this.m_partition; }
      set { this.m_partition = value; }
    }

    /// <summary>
    /// Gets or sets the partition card id.
    /// </summary>
    /// <value>The partition card id.</value>
    public int PartitionCardId
    {
      get { return this.m_partitionCardId; }
      set { this.m_partitionCardId = value; }
    }

    /// <summary>
    /// Gets or sets Partition_start_moment
    /// eenheid: year
    /// </summary>
    /// <value>Partition_start_moment.</value>
    public int Partition_start_moment
    {
      get { return this.m_partition_start_moment; }
      set { this.m_partition_start_moment = value; }
    }

    /// <summary>
    /// Gets or sets Partition_end_moment
    /// eenheid: year
    /// </summary>
    /// <value>Partition_end_moment.</value>
    public int Partition_end_moment
    {
      get { return this.m_partition_end_moment; }
      set { this.m_partition_end_moment = value; }
    }

    /// <summary>
    /// Gets or sets Partition_no_periods
    /// eenheid: -
    /// </summary>
    /// <value>The number.</value>
    public int Partition_no_periods
    {
      get { return this.m_partition_no_periods; }
      set { this.m_partition_no_periods = value; }
    }

    /// <summary>
    /// Gets or sets Partition_pct
    /// eenheid: %
    /// </summary>
    /// <value>The Partition_pct.</value>
    public double Partition_pct
    {
      get { return this.m_partition_pct; }
      set { this.m_partition_pct = value; }
    }

    #endregion Properties ----------------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="Partitions"/> class.
    /// </summary>
    public Partition()
    {
      this.m_partition = string.Empty;
      this.m_partition_start_moment = 0;
      this.m_partition_end_moment = 0;
      this.m_partition_no_periods = 0;
      this.m_partition_pct = 0.0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Partitions"/> class.
    /// </summary>
    /// <param name="partition">The partition.</param>
    /// <param name="partition_start_moment">The partition_start_moment.</param>
    /// <param name="partition_end_moment">The partition_end_moment.</param>
    /// <param name="partition_no_periods">The partition_no_periods.</param>
    /// <param name="partition_pct">The partition_pct.</param>
    public Partition(string partition, int partition_start_moment, int partition_end_moment
      , int partition_no_periods, double partition_pct)
    {
      this.m_partition = partition;
      this.m_partition_start_moment = partition_start_moment;
      this.m_partition_end_moment = partition_end_moment;
      this.m_partition_no_periods = partition_no_periods;
      this.m_partition_pct = partition_pct;
    }

    #endregion Constructors --------------------------------------------------
  }

  /// <summary>
  /// var: ??????? (lijst met Partitions)
  /// </summary>
  [Serializable]
  public class Partitions : List<Partition>, IDeclarations
  {
    /// <summary>
    /// Registers the variabelen.
    /// </summary>
    /// <param name="project">The project.</param>
    /// <returns></returns>
    public bool RegisterVariabelen(Project project)
    {
      try
      {
        string[] partition = new string[this.Count];
        int[] partition_start_moment = new int[this.Count];
        int[] partition_end_moment = new int[this.Count];
        int[] partition_no_periods = new int[this.Count];
        double[] partition_pct = new double[this.Count];

        int teller = 0;

        foreach (Partition partitionItem in this)
        {
          partition[teller] = partitionItem.PartitionName;
          partition_start_moment[teller] = partitionItem.Partition_start_moment;
          partition_end_moment[teller] = partitionItem.Partition_end_moment;
          partition_no_periods[teller] = partitionItem.Partition_no_periods;
          partition_pct[teller++] = partitionItem.Partition_pct;
        }

        object Partitions = partition as object;
        project.AssignElementArray("Partitions", ref Partitions, REPLACE_MODE.REPLACE);

        // Ophalen Segments index
        Set setPartitions = project.GetSet("Partitions");
        int[] partitions = new int[setPartitions.Card];

        object PartitionsUit = partitions as object;
        setPartitions.CompoundTuplePassMode = ELEMENT_PASS_MODE.ELEMENT_BY_NUMBER;
        setPartitions.RetrieveElementArray(ref PartitionsUit);
        partitions = PartitionsUit as int[];

        // Offset op 0 zetten
        setPartitions.OrdinalsOffset = 0;

        for (int index = 0; index < setPartitions.Card; index++)
        {
          string naam = setPartitions.OrdinalToName(index);
          Partition partitionItem;
          if ((partitionItem = this.getPartitionByName(naam)) != null)
          {
            partitionItem.PartitionCardId = partitions[index];
          }
        }

        // Andere parameters doorgeven
        object Partition_start_moment = partition_start_moment as object;
        project.AssignArray("Partition_start_moment", ref Partition_start_moment, 0);

        object Partition_end_moment = partition_end_moment as object;
        project.AssignArray("Partition_end_moment", ref Partition_end_moment, 0);

        object Partition_no_periods = partition_no_periods as object;
        project.AssignArray("Partition_no_periods", ref Partition_no_periods, 0);

        object Partition_pct = partition_pct as object;
        project.AssignArray("Partition_pct", ref Partition_pct, 0);

        return true;
      }
      catch (Exception)
      {
        return false;
      }

    }

    /// <summary>
    /// Gets the partition by name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    private Partition getPartitionByName(string name)
    {
      foreach (Partition partition in this)
      {
        if (partition.PartitionName == name)
        {
          return partition;
        }
      }
      // Niets gevonden
      return null;
    }
  }
}
