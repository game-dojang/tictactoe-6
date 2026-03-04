using UnityEngine;
using System.Threading.Tasks;
using System;
public class OmokAI : MonoBehaviour
{
    private const int MAX_DEPTH = 2;
    private const float MAX_SCORE = 100000;
    private const float OPEN_FOUR = 50000;
    private const float CLOSED_FOUR = 10000;
    private const float OPEN_THREE = 5000;
    private const float CLOSED_THREE = 1000;
    private const float OPEN_TWO = 100;
    private const float CLOSED_TWO = 10;
    
    // 방어 가중치
    private const float DEFENSE_MULTIPLIER = 2f;
    
    /// <summary>
    /// AI(PlayerB)의 최적 수를 비동기로 계산하는 함수
    /// 보드의 모든 유효한 빈 칸에 대해 Minimax를 실행하고, 가장 높은 점수를 가진 위치를 반환한다.
    /// 백그라운드 스레드(Task.Run)에서 실행되어 Unity 메인 스레드를 블로킹하지 않는다.
    /// </summary>
    /// <param name="board">현재 게임 보드 상태 (15x15 PlayerType 배열)</param>
    /// <returns>최적의 (row, col) 좌표. 둘 곳이 없으면 null 반환 (무승부)</returns>
    public static async Task<(int row, int col)?> GetBestMove(Constants.PlayerType[,] board)
    {
        return await Task.Run(() =>
        {
            float bestScore = -MAX_SCORE;
            (int row, int col)? bestMove = null;
        
            float alpha = -MAX_SCORE;
            float beta = MAX_SCORE;
        
            for (var row = 0; row < board.GetLength(0); row++)
            {
                for (var col = 0; col < board.GetLength(1); col++)
                {
                    if (board[row, col] == Constants.PlayerType.None && IsMinimaxValidPosition(row, col, board))
                    {
                        board[row, col] = Constants.PlayerType.Player2;
                        var score = DoMinimax(board, 0, false, alpha, beta);
                        
                        PrintBoard(board);
                        
                        board[row, col] = Constants.PlayerType.None;
                    
                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestMove = (row, col);
                        }
                        alpha = Math.Max(alpha, bestScore);
                    }
                }
            }
        
            return bestMove;
        });

    }

    /// <summary>
    /// 알파-베타 가지치기를 적용한 Minimax 재귀 탐색 함수
    /// isMaximizing=true일 때 AI(PlayerB)의 턴으로 점수를 최대화하고,
    /// isMaximizing=false일 때 상대(PlayerA)의 턴으로 점수를 최소화한다.
    /// 종료 조건: 어느 한 쪽 승리, 무승부(보드 가득 참), 또는 최대 탐색 깊이(MAX_DEPTH) 도달.
    /// 깊이 도달 시 EvaluateBoard로 현재 보드 상태의 휴리스틱 점수를 반환한다.
    /// </summary>
    /// <param name="board">현재 보드 상태 (함수 내에서 직접 수정 후 복원하는 방식으로 사용)</param>
    /// <param name="depth">현재 탐색 깊이 (0부터 시작)</param>
    /// <param name="isMaximizing">true면 AI(최대화) 턴, false면 상대(최소화) 턴</param>
    /// <param name="alpha">알파 값 (최대화 플레이어의 현재 최선 보장 점수)</param>
    /// <param name="beta">베타 값 (최소화 플레이어의 현재 최선 보장 점수)</param>
    /// <returns>해당 보드 상태의 평가 점수 (양수: AI 유리, 음수: 상대 유리)</returns>
    private static float DoMinimax(Constants.PlayerType[,] board, int depth, bool isMaximizing, float alpha, float beta)
    {
        if (CheckGameWin(Constants.PlayerType.Player1, board))
            return -MAX_SCORE;
        if (CheckGameWin(Constants.PlayerType.Player2, board))
            return MAX_SCORE;
        if (IsAllBlocksPlaced(board))
            return 0;
        if (depth >= MAX_DEPTH)
            return EvaluateBoard(board);   // TODO: 평가함수 구현
        
        Debug.Log("DoMinimax: " + depth + " " + isMaximizing + " " + alpha + " " + beta + "");

        if (isMaximizing)
        {
            var bestScore = -MAX_SCORE;
            for (var row = 0; row < board.GetLength(0); row++)
            {
                for (var col = 0; col < board.GetLength(1); col++)
                {
                    if (board[row, col] == Constants.PlayerType.None && IsMinimaxValidPosition(row, col, board))
                    {
                        board[row, col] = Constants.PlayerType.Player2;
                        var score = DoMinimax(board, depth + 1, false, alpha, beta);

                        PrintBoard(board);
                        
                        board[row, col] = Constants.PlayerType.None;
                        bestScore = Math.Max(bestScore, score);
                        alpha = Math.Max(alpha, bestScore);
                        if (beta <= alpha)
                            break;
                    }
                }
                if (beta <= alpha)
                    break;
            }
            return bestScore;
        }
        else
        {
            var bestScore = MAX_SCORE;
            for (var row = 0; row < board.GetLength(0); row++)
            {
                for (var col = 0; col < board.GetLength(1); col++)
                {
                    if (board[row, col] == Constants.PlayerType.None && IsMinimaxValidPosition(row, col, board))
                    {
                        board[row, col] = Constants.PlayerType.Player1;
                        var score = DoMinimax(board, depth + 1, true, alpha, beta);
                        
                        PrintBoard(board);

                        board[row, col] = Constants.PlayerType.None;
                        bestScore = Math.Min(bestScore, score);     // B
                        beta = Math.Min(beta, bestScore);
                        if (beta <= alpha)
                            break;
                    }
                }
                if (beta <= alpha)
                    break;
            }
            return bestScore;
        }
    }
    
    /// <summary>
    /// 보드 전체의 휴리스틱 점수를 계산하는 평가 함수
    /// 모든 칸에서 가로, 세로, 대각선(우하향), 대각선(우상향) 4방향으로 패턴을 분석한다.
    /// AI(PlayerB) 돌의 패턴은 양수로, 상대(PlayerA) 돌의 패턴은 음수에 방어 가중치(DEFENSE_MULTIPLIER=2)를
    /// 곱하여 합산한다. 방어 가중치가 높아 AI는 공격보다 상대 위협 차단을 우선시한다.
    /// </summary>
    /// <param name="board">평가할 보드 상태</param>
    /// <returns>보드의 총 평가 점수 (양수: AI 유리, 음수: 상대 유리)</returns>
    private static float EvaluateBoard(Constants.PlayerType[,] board)
    {
        float score = 0;
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        // 각 방향에 대한 패턴 평가
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (board[row, col] == Constants.PlayerType.None)
                    continue;

                bool isAI = board[row, col] == Constants.PlayerType.Player2;
                float multiplier = isAI ? 1 : -DEFENSE_MULTIPLIER;

                // 가로 방향
                if (col <= cols - 5)
                {
                    var pattern = EvaluatePattern(board, row, col, 0, 1, 5);
                    score += pattern * multiplier;
                }

                // 세로 방향
                if (row <= rows - 5)
                {
                    var pattern = EvaluatePattern(board, row, col, 1, 0, 5);
                    score += pattern * multiplier;
                }

                // 대각선 방향 (우하향)
                if (row <= rows - 5 && col <= cols - 5)
                {
                    var pattern = EvaluatePattern(board, row, col, 1, 1, 5);
                    score += pattern * multiplier;
                }

                // 대각선 방향 (우상향)
                if (row >= 4 && col <= cols - 5)
                {
                    var pattern = EvaluatePattern(board, row, col, -1, 1, 5);
                    score += pattern * multiplier;
                }
            }
        }

        return score;
    }

    /// <summary>
    /// 특정 위치에서 지정된 방향으로 연속된 돌의 패턴을 분석하고 점수를 반환하는 함수
    /// 시작 위치의 돌과 같은 종류의 연속된 돌 개수를 세고, 양쪽 끝의 열린/막힌 상태를 확인한다.
    /// - 5개 이상 연속: MAX_SCORE (승리)
    /// - 4개 연속: 양쪽 열림(OPEN_FOUR) / 한쪽 막힘(CLOSED_FOUR) / 완전 막힘(CLOSED_FOUR/2)
    /// - 3개 연속: 양쪽 열림(OPEN_THREE) / 한쪽 막힘(CLOSED_THREE) / 완전 막힘(CLOSED_THREE/2)
    /// - 2개 연속: 양쪽 열림(OPEN_TWO) / 한쪽 막힘(CLOSED_TWO) / 완전 막힘(CLOSED_TWO/2)
    /// </summary>
    /// <param name="board">현재 보드 상태</param>
    /// <param name="startRow">패턴 시작 행 좌표</param>
    /// <param name="startCol">패턴 시작 열 좌표</param>
    /// <param name="dRow">행 방향 증분 (-1, 0, 1)</param>
    /// <param name="dCol">열 방향 증분 (-1, 0, 1)</param>
    /// <param name="length">탐색할 최대 길이</param>
    /// <returns>패턴의 위협도 점수</returns>
    private static float EvaluatePattern(Constants.PlayerType[,] board, int startRow, int startCol, int dRow, int dCol, int length)
    {
        var currentPlayer = board[startRow, startCol];
        int count = 1;
        int emptyBefore = 0;
        int emptyAfter = 0;
        bool blocked = false;

        // 연속된 돌 확인
        for (int i = 1; i < length; i++)
        {
            int newRow = startRow + dRow * i;
            int newCol = startCol + dCol * i;

            if (!IsValidPosition(newRow, newCol, board))
            {
                blocked = true;
                break;
            }

            if (board[newRow, newCol] == currentPlayer)
            {
                count++;
            }
            else if (board[newRow, newCol] == Constants.PlayerType.None)
            {
                emptyAfter++;
                break;
            }
            else
            {
                blocked = true;
                break;
            }
        }

        // 반대 방향 빈 공간 확인
        for (int i = 1; i < length; i++)
        {
            int newRow = startRow - dRow * i;
            int newCol = startCol - dCol * i;

            if (!IsValidPosition(newRow, newCol, board))
            {
                blocked = true;
                break;
            }

            if (board[newRow, newCol] == Constants.PlayerType.None)
            {
                emptyBefore++;
                break;
            }
            else if (board[newRow, newCol] != currentPlayer)
            {
                blocked = true;
                break;
            }
        }

        // 패턴 점수 계산
        if (count >= 5) return MAX_SCORE;
        
        bool isOpen = emptyBefore > 0 && emptyAfter > 0;
        
        switch (count)
        {
            case 4:
                return isOpen ? OPEN_FOUR : (blocked ? CLOSED_FOUR / 2 : CLOSED_FOUR);
            case 3:
                return isOpen ? OPEN_THREE : (blocked ? CLOSED_THREE / 2 : CLOSED_THREE);
            case 2:
                return isOpen ? OPEN_TWO : (blocked ? CLOSED_TWO / 2 : CLOSED_TWO);
            default:
                return 0;
        }
    }

    /// <summary>
    /// 주어진 좌표가 보드 범위 안에 있는지 확인하는 함수
    /// </summary>
    /// <param name="row">확인할 행 좌표</param>
    /// <param name="col">확인할 열 좌표</param>
    /// <param name="board">현재 보드</param>
    /// <returns>범위 안이면 true, 밖이면 false</returns>
    private static bool IsValidPosition(int row, int col, Constants.PlayerType[,] board)
    {
        return row >= 0 && row < board.GetLength(0) && col >= 0 && col < board.GetLength(1);
    }

    /// <summary>
    /// Minimax 탐색 시 해당 위치가 탐색할 가치가 있는지 판단하는 함수
    /// 8방향(상하좌우 + 대각선 4방향) 인접 칸 중 하나라도 돌이 놓여있으면 true를 반환한다.
    /// 기존 돌과 완전히 떨어진 고립된 빈 칸은 탐색에서 제외하여 불필요한 연산을 줄인다.
    /// </summary>
    /// <param name="row">확인할 행 좌표</param>
    /// <param name="col">확인할 열 좌표</param>
    /// <param name="board">현재 보드</param>
    /// <returns>인접한 돌이 있으면 true (탐색 대상), 없으면 false (탐색 제외)</returns>
    private static bool IsMinimaxValidPosition(int row, int col, Constants.PlayerType[,] board)
    {
        if ((row > 0 && col > 0 && board[row - 1, col - 1] != Constants.PlayerType.None) ||
            (row > 0 && board[row - 1, col] != Constants.PlayerType.None) ||
            (row > 0 && col < board.GetLength(1) - 1 && board[row - 1, col + 1] != Constants.PlayerType.None) ||
            (col > 0 && board[row, col - 1] != Constants.PlayerType.None) ||
            (col < board.GetLength(1) - 1 && board[row, col + 1] != Constants.PlayerType.None) ||
            (row < board.GetLength(0) - 1 && col > 0 && board[row + 1, col - 1] != Constants.PlayerType.None) ||
            (row < board.GetLength(0) - 1 && board[row + 1, col] != Constants.PlayerType.None) ||
            (row < board.GetLength(0) - 1 && col < board.GetLength(1) - 1 && board[row + 1, col + 1] != Constants.PlayerType.None))
        {
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 모든 마커가 보드에 배치 되었는지 확인하는 함수
    /// </summary>
    /// <returns>True: 모두 배치</returns>
    public static bool IsAllBlocksPlaced(Constants.PlayerType[,] board)
    {
        for (var row = 0; row < board.GetLength(0); row++)
        {
            for (var col = 0; col < board.GetLength(1); col++)
            {
                if (board[row, col] == Constants.PlayerType.None)
                    return false;
            }
        }
        return true;
    }
    
    /// <summary>
    /// 15x15 오목판에서 승리한 플레이어를 확인하는 함수
    /// </summary>
    /// <param name="playerType">확인할 플레이어 타입</param>
    /// <param name="board">현재 게임 보드</param>
    /// <returns>해당 플레이어가 승리했으면 true, 아니면 false</returns>
    public static bool CheckGameWin(Constants.PlayerType playerType, Constants.PlayerType[,] board)
    {
        // None 타입은 승리 조건에서 제외
        if (playerType == Constants.PlayerType.None)
            return false;

        // 가로 방향 확인
        for (var row = 0; row < board.GetLength(0); row++)
        {
            for (var col = 0; col <= board.GetLength(1) - 5; col++)
            {
                bool win = true;
                for (var i = 0; i < 5; i++)
                {
                    if (board[row, col + i] != playerType)
                    {
                        win = false;
                        break;
                    }
                }
                if (win) 
                {
                    return true;
                }
            }
        }

        // 세로 방향 확인
        for (var row = 0; row <= board.GetLength(0) - 5; row++)
        {
            for (var col = 0; col < board.GetLength(1); col++)
            {
                bool win = true;
                for (var i = 0; i < 5; i++)
                {
                    if (board[row + i, col] != playerType)
                    {
                        win = false;
                        break;
                    }
                }
                if (win) 
                {
                    return true;
                }
            }
        }

        // 대각선 방향 (좌상단 -> 우하단) 확인
        for (var row = 0; row <= board.GetLength(0) - 5; row++)
        {
            for (var col = 0; col <= board.GetLength(1) - 5; col++)
            {
                bool win = true;
                for (var i = 0; i < 5; i++)
                {
                    if (board[row + i, col + i] != playerType)
                    {
                        win = false;
                        break;
                    }
                }
                if (win) 
                {
                    return true;
                }
            }
        }

        // 대각선 방향 (우상단 -> 좌하단) 확인
        for (var row = 0; row <= board.GetLength(0) - 5; row++)
        {
            for (var col = 4; col < board.GetLength(1); col++)
            {
                bool win = true;
                for (var i = 0; i < 5; i++)
                {
                    if (board[row + i, col - i] != playerType)
                    {
                        win = false;
                        break;
                    }
                }
                if (win) 
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 디버그용 보드 출력 함수
    /// 현재 보드 상태를 콘솔에 텍스트로 시각화한다.
    /// PlayerA는 [o], PlayerB는 [x], 빈 칸은 [ ]로 표시된다.
    /// </summary>
    /// <param name="board">출력할 보드 상태</param>
    public static void PrintBoard(Constants.PlayerType[,] board)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        string output = "\n";
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                switch (board[row, col])
                {
                    case Constants.PlayerType.Player1:
                        output += "[o]";
                        break;
                    case Constants.PlayerType.Player2:
                        output += "[x]";
                        break;
                    default:
                        output += "[ ]";
                        break;
                }
            }
            output += "\n";
        }
        Debug.Log(output);
    }
}
