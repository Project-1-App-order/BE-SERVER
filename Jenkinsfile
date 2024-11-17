pipeline {
    agent any

    environment {
        DOCKER_COMPOSE_FILE = 'docker-compose.yml' // Tên file Docker Compose
        PROJECT_PATH = '/path/to/your/project'     // Đường dẫn nơi project được lưu trên server
    }

    stages {
        stage('Cleanup Old Project') {
            steps {
                script {
                    // Xóa container cũ và volume liên quan
                    sh "docker-compose -f ${DOCKER_COMPOSE_FILE} down -v"
                    
                    // Xóa mã nguồn cũ
                    sh "rm -rf ${PROJECT_PATH}/*"
                }
            }
        }

        stage('Checkout New Code') {
            steps {
                script {
                    // Lấy mã nguồn mới từ repository
                    git branch: 'main', url: 'https://github.com/Project-1-App-order/BE-SERVER.git'
                }
            }
        }

        stage('Build and Deploy New Project') {
            steps {
                script {
                    // Build lại và chạy Docker Compose
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
