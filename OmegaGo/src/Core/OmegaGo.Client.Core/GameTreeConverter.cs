﻿using System;
using System.Collections.Generic;
using System.Linq;
using OmegaGo.Core.Sgf;
using OmegaGo.Core.Sgf.Properties.Values;
using OmegaGo.Core.Sgf.Properties.Values.ValueTypes;

namespace OmegaGo.Core
{
    /// <summary>
    /// Converting between Game Trees and SGF Game trees
    /// </summary>
    public static class GameTreeConverter
    {
        /// <summary>
        /// Converts a SGF game tree to GameTree
        /// </summary>
        /// <param name="tree">SGF game tree</param>
        /// <returns>Game tree</returns>
        public static GameTreeNode FromSgfGameTree(SgfGameTree tree) => ConvertBranch(tree);

        /// <summary>
        /// Converts a SGF tree branch to GameTreeNode
        /// </summary>
        /// <param name="branch">Branch</param>
        /// <returns>Game tree node</returns>
        private static GameTreeNode ConvertBranch(SgfGameTree branch)
        {
            GameTreeNode root = null;
            GameTreeNode current = null;
            foreach (var node in branch.Sequence)
            {
                GameTreeNode newNode = null;
                if (node["W"] != null)
                {
                    SgfPoint point = node["W"].Value<SgfPoint>();
                    //add white move
                    newNode = new GameTreeNode(Move.PlaceStone(StoneColor.White, Position.FromSgfPoint(point)));
                }
                else if (node["B"] != null)
                {
                    SgfPoint point = node["B"].Value<SgfPoint>();
                    //add black move
                    newNode = new GameTreeNode(Move.PlaceStone(StoneColor.Black, Position.FromSgfPoint(point)));
                }
                else
                {
                    //add non-move
                    newNode = new GameTreeNode(Move.NoneMove);
                }
                if (node["AW"] != null)
                {
                    //add white moves
                    var property = node["AW"];
                    var pointRectangles = property.Values<SgfPointRectangle>();                    
                    newNode.AddWhite.AddRange( GetPositionsFromPointRectangles( pointRectangles ) );
                }
                if (node["AB"] != null)
                {
                    var property = node["AB"];
                    var pointRectangles = property.Values<SgfPointRectangle>();
                    newNode.AddBlack.AddRange(GetPositionsFromPointRectangles(pointRectangles));
                }
                if ( node[ "C" ] != null )
                {
                    var property = node[ "C" ];
                    var comment = property.Value<string>();
                    newNode.Comment = comment;
                }
                if (node ["GN"] != null)
                {
                    var property = node["GN"];
                    var gameName = property.Value<string>();
                    newNode.Name = gameName;
                }
                if (current == null)
                {
                    root = newNode;
                }
                else
                {
                    current.Branches.AddNode(newNode);
                }
                current = newNode;
            }
            //create root if none were found
            if (root == null)
            {
                root = new GameTreeNode(Move.NoneMove);
                current = root;
            }
            //add branches
            foreach (var child in branch.Children)
            {
                var rootOfBranch = ConvertBranch(child);
                current.Branches.AddNode(rootOfBranch);
            }
            return root;
        }

        private static IEnumerable<Position> GetPositionsFromPointRectangles(IEnumerable<SgfPointRectangle> pointRectangles)
        {
            foreach (var pointRectangle in pointRectangles)
            {
                foreach (var point in pointRectangle)
                {
                    yield return Position.FromSgfPoint(point);
                }
            }
        }


        /// <summary>
        /// Converts a GameTree to SGF game tree
        /// </summary>
        /// <returns>SGF game tree</returns>
        public static SgfGameTree ToSgfGameTree(GameTree tree)
        {
            throw new NotImplementedException();
        }
    }
}
