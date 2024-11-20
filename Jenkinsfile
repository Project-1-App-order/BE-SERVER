pipeline {
    agent any

    environment {
        DOCKER_COMPOSE_FILE = 'docker-compose.yml'
    }

    stages {
        stage('Build and Deploy New Project') {
            steps {
                script {
                    sh """
                    
                    docker-compose up -d --build
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
