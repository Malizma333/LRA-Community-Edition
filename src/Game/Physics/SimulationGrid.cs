//
//  SimulationGrid.cs
//
//  Author:
//       Noah Ablaseau <nablaseau@hotmail.com>
//
//  Copyright (c) 2017 
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using linerider.Rendering;
using OpenTK;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using linerider.Game;
using linerider.Utils;
using linerider.Lines;

namespace linerider
{

    public partial class SimulationGrid : ISimulationGrid
    {
        public const int CellSize = 14;
        public int GridVersion = 62;
        public ResourceSync Sync
        {
            get { return _sync; }
        }
        private readonly Dictionary<int, SimulationCell> Cells = new Dictionary<int, SimulationCell>(4096);
        private readonly ResourceSync _sync = new ResourceSync();

        public List<CellLocation> GetGridPositions(StandardLine line)
        {
            return GetGridPositions(line, GridVersion);
        }
        public void AddLine(StandardLine line)
        {
            var positions = GetGridPositions(line);
            using (_sync.AcquireWrite())
            {
                foreach (var pos in positions)
                {
                    Register(line, pos.X, pos.Y);
                }
            }
        }
        public void RemoveLine(StandardLine line)
        {
            var positions = GetGridPositions(line);
            using (_sync.AcquireWrite())
            {
                foreach (var pos in positions)
                {
                    Unregister(line, pos.X, pos.Y);
                }
            }
        }
        public void MoveLine(Vector2d p1, Vector2d p2, StandardLine line)
        {
            var oldpos = GetGridPositions(p1, p2, GridVersion);
            var newpos = GetGridPositions(line);
           // foreach (var v in oldpos)
            {
         //       if (!newhash.Contains(v))
       //             remove.Add(v);
            }
         //   foreach (var v in newpos)
         //   {
         //       if (!oldhash.Contains(v))
             //       add.Add(v);
           // }
            using (_sync.AcquireWrite())
            {
                foreach (var v in oldpos)
                {
                    Unregister(line, v.X, v.Y);
                }
                foreach (var v in newpos)
                {
                    Register(line, v.X, v.Y);
                }
            }
        }
        public virtual SimulationCell GetCell(int x, int y)
        {
            SimulationCell cell;
            var pos = GetCellKey(x, y);
            if (!Cells.TryGetValue(pos, out cell))
                return null;
            return cell;

        }

        public SimulationCell PointToChunk(Vector2d pos)
        {
            return GetCell((int)Math.Floor(pos.X / CellSize), (int)Math.Floor(pos.Y / CellSize));
        }

        protected int GetCellKey(int x, int y)
        {
            unchecked
            {
                int hash = 27;
                hash = hash * 486187739 + x;
                hash = hash * 486187739 + y;
                return hash;
            }
        }
        private void Register(StandardLine l, int x, int y)
        {
            var key = GetCellKey(x, y);
            SimulationCell cell;
            if (!Cells.TryGetValue(key, out cell))
            {
                cell = new SimulationCell();
                Cells[key] = cell;
            }
            cell.AddLine(l);
        }

        private void Unregister(StandardLine l, int x, int y)
        {
            SimulationCell cell;
            var pos = GetCellKey(x, y);
            if (!Cells.TryGetValue(pos, out cell))
                return;
            cell.RemoveLine(l.ID);
        }
    }
}