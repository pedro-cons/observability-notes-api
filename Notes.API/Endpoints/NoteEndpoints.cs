using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notes.API.Database;
using Notes.API.Entities;
using Notes.API.Entities.Request;

namespace Notes.API.Endpoints;

public static class NoteEndpoints
{
    public static void MapNoteEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("notes", async (
            [FromBody] CreateNoteRequest request, 
            AppDbContext context,
            ILoggerFactory loggerFactory,
            CancellationToken ct) =>
        {
            var logger = loggerFactory.CreateLogger("NoteEndpoints");

            try
            {
                if (string.IsNullOrWhiteSpace(request.Description))
                {
                    logger.LogWarning("Invalid description -> {Description}", request.Description);

                    return Results.BadRequest();
                }

                var note = new Note()
                {
                    Description = request.Description,
                };

                await context.AddAsync(note);
                await context.SaveChangesAsync(ct);

                logger.LogInformation("Note created with ID -> {Id}", note.Id);

                return Results.Ok(note);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating note -> {Message}", ex.Message);

                return Results.BadRequest();
            }
        });

        app.MapGet("notes", async (
            AppDbContext context, 
            CancellationToken ct, 
            int page = 1, 
            int pageSize = 10) =>
        {
            var notes = await context.Notes
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return Results.Ok(notes);
        });
    }
}
