using ChessChallenge.API;
using System;

namespace ChessChallenge.Example
{
    // A simple bot that can spot mate in one, and always captures the most valuable piece it can.
    // Plays randomly otherwise.
    public class EvilBot : IChessBot
    {
        Board board;

        public Move Think(Board inBoard, Timer inTimer)
        {
            board = inBoard;

            Span<Move> moves = board.GetLegalMoves().AsSpan();
            int count = moves.Length;

            if (count == 1)
                return moves[0];

            //int alpha = -99999;
            //int beta = 99999;

            int bestMoveIndex = 0;
            int bestMoveEval = board.IsWhiteToMove ? -99999 : 99999;
            int eval = 0;


            for (int i = 0; i < count; i++)
            {
                Move move = moves[i];
                board.MakeMove(move);
                eval = AlphaBeta(2, board.IsWhiteToMove, int.MinValue, int.MaxValue);//(board.playerTurn == 8) ? true : false
                board.UndoMove(move);

                if (board.IsWhiteToMove) // max
                {
                    if (eval > bestMoveEval)
                    {
                        bestMoveEval = eval;
                        bestMoveIndex = i;
                    }
                }
                else    // min
                {
                    if (eval < bestMoveEval)
                    {
                        bestMoveEval = eval;
                        bestMoveIndex = i;
                    }
                }
            }

            return moves[bestMoveIndex];
        }

        int AlphaBeta(int depth, bool maxPlayer, int alpha, int beta)
        {
            if (depth == 0)
                return Evalutate();

            Span<Move> moves = board.GetLegalMoves().AsSpan();
            int Count = moves.Length;

            if (Count == 0) // Later make captures
                return Evalutate();

            if (maxPlayer)
            {
                int maxEval = int.MinValue;
                foreach (Move move in moves)
                {
                    board.MakeMove(move);
                    int eval = AlphaBeta(depth - 1, false, alpha, beta);
                    board.UndoMove(move);
                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);
                    if (beta >= alpha)
                        break;
                }
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                foreach (Move move in moves)
                {
                    board.MakeMove(move);
                    int eval = AlphaBeta(depth - 1, true, alpha, beta);
                    board.UndoMove(move);
                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);
                    if (alpha >= beta)
                        break;
                }
                return minEval;
            }
        }

        int Evalutate()
        {
            if (board.IsInCheckmate())
                return board.IsWhiteToMove ? -99999 : 99999;

            PieceList[] pieces = board.GetAllPieceLists();

            return (pieces[0].Count * 100 + pieces[1].Count * 300 + pieces[2].Count * 300 + pieces[3].Count * 500 + pieces[4].Count * 900) -
                (pieces[6].Count * 100 + pieces[7].Count * 300 + pieces[8].Count * 300 + pieces[9].Count * 500 + pieces[10].Count * 900);
        }
    }
}