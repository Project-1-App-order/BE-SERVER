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
        stage('Build and Deploy New Project') {
            steps {
                script {
                    sh """
                    docker-compose down
                    sh "docker ps"
                    docker-compose up -d --build
                    """
                }
            }
        }

        stage('Check Docker Compose Version') {
            steps {
                script {
                    sh "docker ps"
                    sh "docker ps -a"
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
