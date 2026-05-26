# 1. Установка ML-Agents
pip install mlagents

# 2. Запуск обучения с нуля
mlagents-learn drone_config.yaml --run-id=drone_run_01

# 3. Возобновление прерванного обучения
mlagents-learn drone_config.yaml --run-id=drone_run_01 --resume

# 4. Обучение с подражанием (имитация)
mlagents-learn drone_config.yaml --run-id=imitation_run --initialize-from=expert_demo.demo

# 5. Визуализация TensorBoard
tensorboard --logdir results
