using ChessChallenge.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

public class MyBot : IChessBot
{
    Board board;
    Timer timer;
    int timeToThink;
    bool stopped;

    public Move Think(Board inBoard, Timer inTimer)
    {
        board = inBoard;
        timer = inTimer;
        timeToThink = inTimer.MillisecondsRemaining / 100;
        stopped = false;



        Move[] movesRef = board.GetLegalMoves(); // Used for Array.Sort()
        Span<Move> moves = movesRef.AsSpan();
        int movesCount = moves.Length;

        OrderMoves(moves);

        Stopwatch thinkTime = Stopwatch.StartNew();

        int DepthReached = 2; // Skip 1 depth since we move order
        int bestMoveIndex = 0;

        while (true)
        {
            DepthReached++;
            var results = Search(DepthReached, ref moves, movesCount, bestMoveIndex);

            bestMoveIndex = results.BestMoveIndex;
            //Console.WriteLine();
            if (results.Finished)
            {

                bestMoveIndex = results.BestMoveIndex;

                Array.Sort(results.Values, movesRef); // Should also order the Span moves...
            }
            else
            {
                //Array.Copy(results.Values, be);
                break;
            }

            if (results.Values[results.BestMoveIndex] > 90000 || results.Values[results.BestMoveIndex] < -90000)
                break; // Somebody is in trouble
        }

        Console.WriteLine(DepthReached);

        return moves[bestMoveIndex];
    }

    /// <summary>
    /// Return the scores of the difrent moves, and take in the moves 
    /// </summary>
    public (int[] Values, bool Finished, int BestMoveIndex, int MovesFinished) Search(int depth, ref Span<Move> moves, int moveCount, int previousBestMove)
    {
        // NOTE
        // Also use alpha beta pruning in HERE
        // And also make sure the values/moves get sorted the right way since black and white wants difrent values


        int bestMove = 0,
            eval,
            bestMoveEval = HighOrLow();

        int[] values = new int[moveCount];
        for (int i = 0; i < moveCount; i++)
        {
            if (stopped)
                return (values, false, previousBestMove, i - 1);

            board.MakeMove(moves[i]);
            eval = AlphaBeta(depth - 1, board.IsWhiteToMove, -99999, 99999, false);
            board.UndoMove(moves[i]);


            if (board.IsWhiteToMove) // max
            {
                if (eval > bestMoveEval && eval != bestMoveEval)
                {
                    bestMoveEval = eval;
                    bestMove = i;
                }
            }
            else    // min
            {
                if (eval < bestMoveEval && eval != bestMoveEval)
                {
                    bestMoveEval = eval;
                    bestMove = i;
                }
            }
            values[i] = eval;
        }

        return (values, true, bestMove, moveCount);
    }

    public int AlphaBeta(int depth, bool maxPlayer, int alpha, int beta, bool capturesOnly) // Maby make all the ints into a parems[]? for more keywords
    {
        if (depth == 0)
            return AlphaBeta(-1, maxPlayer, alpha, beta, true);

        if (timeToThink < timer.MillisecondsElapsedThisTurn)
        {
            stopped = true;
            return 0;
        }

        if (board.IsInCheckmate())
            return HighOrLow();// 0 moves

        Move[] moves = board.GetLegalMoves(capturesOnly);
        int TotalCount = moves.Count();

        if (capturesOnly && TotalCount == 0)
            return Evaluate();

        int eval;

        if (maxPlayer)
        {
            int maxEval = -99999;
            foreach (Move move in moves)
            {
                board.MakeMove(move);
                eval = AlphaBeta(depth - 1, false, alpha, beta, capturesOnly);
                board.UndoMove(move);
                maxEval = Math.Max(maxEval, eval);
                alpha = Math.Max(alpha, eval);
                if (beta <= alpha)
                {
                    break;
                }
            }

            return maxEval;
        }
        else
        {
            int minEval = 99999;
            foreach (Move move in moves)
            {
                board.MakeMove(move);
                eval = AlphaBeta(depth - 1, true, alpha, beta, capturesOnly);
                board.UndoMove(move);
                minEval = Math.Min(minEval, eval);
                beta = Math.Min(beta, eval);
                if (beta <= alpha)
                {
                    break;
                }
            }

            return minEval;
        }
    }

    int HighOrLow() => board.IsWhiteToMove ? -99999 : 99999;

    int Evaluate()
    {
        PieceList[] pieces = board.GetAllPieceLists();

        return (pieces[0].Count * 100 + pieces[1].Count * 300 + pieces[2].Count * 300 + pieces[3].Count * 500 + pieces[4].Count * 900) -
            (pieces[6].Count * 100 + pieces[7].Count * 300 + pieces[8].Count * 300 + pieces[9].Count * 500 + pieces[10].Count * 900);
    }

    void OrderMoves(Span<Move> moves)
    {
        moves[0] = moves[0];
    }
}