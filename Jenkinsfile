pipeline {
    agent any

    environment {
        DOCKER_COMPOSE_FILE = 'docker-compose.yml'
        PROJECT_PATH = '/data'     
    }

    stages {
        stage('Build and Deploy New Project') {
            steps {
                script {
                    sh """
                    docker-compose -f ${DOCKER_COMPOSE_FILE} up -d --build
                    """
                }
            }
        }
    }

    post {
        always {
            echo 'Pipeline completed!'
        }
        success {
            echo 'Project deployed successfully!'
        }
        failure {
            echo 'Deployment failed!'
        }
    }
}
