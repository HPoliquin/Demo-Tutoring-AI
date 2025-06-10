# Use the official PostgreSQL image from Docker Hub
FROM postgres:15.3

# Set environment variables for PostgreSQL
ENV POSTGRES_USER=${POSTGRES_USER}
ENV POSTGRES_PASSWORD=${POSTGRES_PASSWORD}
ENV POSTGRES_DB=${POSTGRES_DB}

# Expose the default PostgreSQL port
EXPOSE 5432

# Use a volume to persist data
VOLUME ["/var/lib/postgresql/data"]