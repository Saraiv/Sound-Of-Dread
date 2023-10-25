using UnityEngine;

/*
    aqui e feita a AI do chase player state do monstro
*/

public class AiStateChasePlayer : AiState{
    float timer = 0.0f;
    float wallTimer = 0.0f;
    bool isBehindWall = false;

    public AiStateId GetId(){
        return AiStateId.ChasePlayer;
    }

    public void Enter(AiAgent agent){
        timer = agent.timeForMonsterToStopLooking;
        wallTimer = 1.0f;
        agent.animator.Play("Chase");
    }

    public void Exit(AiAgent agent){

    }

    public void Update(AiAgent agent){
        if (!agent.navMeshAgent.enabled) return;

        bool canSeePlayer = CanSeePlayer(agent);
        if (canSeePlayer) timer = agent.timeForMonsterToStopLooking; // tempo que o inimigo segue desde o momento que nao consegue ver o player
        else timer -= Time.deltaTime;

        wallTimer -= Time.deltaTime;
        if (wallTimer <= 0.0f){
            isBehindWall = IsBehindWall(agent);
            wallTimer = 1.0f; // verifica se esta alguma parede/objeto a frente a cada segundo para ativar o ticker se segue ou nao
        }

        // se nao esta atras da parede simplesmente segue
        if (!isBehindWall) agent.navMeshAgent.destination = agent.playerTranform.position;

        // se a distancia for a pretendida, ou seja se o AI consegue ver o jogador e se estiver no range para tal
        // entao diminuir o timer
        if (Vector3.Distance(agent.transform.position, agent.playerTranform.position) >= agent.agentView && canSeePlayer)
            timer -= Time.deltaTime;

        if (timer < 0.0f) agent.stateMachine.ChangeState(AiStateId.Patrol); // assim que o timer chegar a 0 o monstro para de se mover e fica no seu estado patrol

        // se a distancia for a pretendida para atacar então muda estado para atacar
        if (Vector3.Distance(agent.transform.position, agent.playerTranform.position) < agent.agentStoppingDistance)
            agent.stateMachine.ChangeState(AiStateId.Attack);
    }

    private static bool CanSeePlayer(AiAgent agent){
        /*
            implementa basicamente um raycast/linha de visao para ver se o inimigo consegue ver o jogador
            retorna verdadeiro se o inimigo consegue ver o jogador e falso se o contrario
        */
        RaycastHit hit;
        if (Physics.Raycast(agent.transform.position, agent.playerTranform.position - agent.transform.position, out hit)
            && Vector3.Distance(agent.transform.position, agent.playerTranform.position) >= agent.agentView)
            if (hit.collider.CompareTag("Player")) return true;
        return false;
    }

    private static bool IsBehindWall(AiAgent agent){
        /*
            implementa basicamente um raycast/linha de visao para ver se o inimigo consegue ver o jogador
            retorna verdadeiro se o jogador estiver atras da parede ou falso se o contrario
        */
        RaycastHit hit;
        if (Physics.Raycast(agent.transform.position, agent.playerTranform.position - agent.transform.position, out hit)
            && Vector3.Distance(agent.transform.position, agent.playerTranform.position) >= agent.agentView)
            if (hit.collider.CompareTag("Wall")) return true;
        return false;
    }
}
