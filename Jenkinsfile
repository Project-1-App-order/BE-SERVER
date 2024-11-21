pipeline {
    agent any

    environment {
        DOCKER_COMPOSE_FILE = 'docker-compose.yml'
    }

    stages {
        stage('Check Docker Version') {
            steps {
                script {
                    sh "docker-compose --version"
                    sh "docker ps"
                    sh "docker ps -a"
                }
            }
        }
        stage('Test') {
            steps {
                script {
                    echo "Running environment tests..."
                }
            }
        }
        stage('Build and Deploy New Project') {
            steps {
                script {
                    sh """
                    docker ps
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
