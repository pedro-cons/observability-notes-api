using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notes.API.Database;
using Notes.API.Diagnostics;
using Notes.API.Entities;
using Notes.API.Entities.Enumerator;
using Notes.API.Entities.Request;

namespace Notes.API.Endpoints;

public static class NoteEndpoints
{
    public static void MapNoteEndpoints(this IEndpointRouteBuilder app)
    {
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

        app.MapGet("notes/{id:int}", async (
           int id,
           AppDbContext context,
           CancellationToken ct) =>
        {
            var note = await context.Notes
                .FirstOrDefaultAsync(n => n.Id == id, ct);

            if (note is null)
            {
                return Results.NotFound($"Note with ID -> {id} not found for update");
            }

            return Results.Ok(note);
        });

        app.MapPost("notes", async (
            [FromBody] CreateNoteRequest request,
            AppDbContext context,
            ILogger<Program> logger,
            CancellationToken ct) =>
        {
            try
            {
                if (request.Description.Length > CNotesLength.description)
                {
                    logger.LogWarning("Invalid description length -> {Description} - {length}", request.Description, request.Description.Length);

                    DiagnosticsConfig.NotesDescriptionTooLong.Add(1,
                                                                  new KeyValuePair<string, object?>("note.description", request.Description),
                                                                  new KeyValuePair<string, object?>("note.description.length", request.Description.Length));

                    return Results.BadRequest("Description is too long.");
                }

                var note = new Note()
                {
                    Description = request.Description,
                };

                await context.AddAsync(note);
                await context.SaveChangesAsync(ct);

                DiagnosticsConfig.NotesCounter.Add(1, new KeyValuePair<string, object?>("note.id", note.Id));

                logger.LogInformation("Note created with ID -> {Id}", note.Id);

                return Results.Ok(note);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating note -> {Message}", ex.Message);

                return Results.BadRequest();
            }
        });

        app.MapDelete("notes/{id:int}", async (
            int id,
            AppDbContext context,
            ILogger<Program> logger,
            CancellationToken ct) =>
        {
            var note = await context.Notes
                .FirstOrDefaultAsync(n => n.Id == id, ct);

            if (note is null)
            {
                logger.LogWarning("Note with ID -> {Id} not found for update", id);

                return Results.NotFound($"Note with ID -> {id} not found for update");
            }

            context.Notes.Remove(note);
            await context.SaveChangesAsync(ct);

            DiagnosticsConfig.NotesCounter.Add(-1, new KeyValuePair<string, object?>("note.id", note.Id));

            logger.LogInformation("Note with ID -> {Id} deleted", id);

            return Results.NoContent();
        });

        app.MapPut("notes/{id:int}", async (
            int id,
            [FromBody] UpdateNoteRequest request,
            AppDbContext context,
            ILogger<Program> logger,
            CancellationToken ct) =>
        {
            var note = await context.Notes
                .FirstOrDefaultAsync(n => n.Id == id, ct);

            if (note is null)
            {
                logger.LogWarning("Note with ID -> {Id} not found for update", id);

                return Results.NotFound($"Note with ID -> {id} not found for update");
            }

            if (request.Description.Length > CNotesLength.description)
            {
                logger.LogWarning("Invalid description length -> {Description} - {length}", request.Description, request.Description.Length);

                DiagnosticsConfig.NotesDescriptionTooLong.Add(1,
                                                              new KeyValuePair<string, object?>("note.description", request.Description),
                                                              new KeyValuePair<string, object?>("note.description.length", request.Description.Length));

                return Results.BadRequest("Description is too long.");
            }

            note.Description = request.Description;

            context.Notes.Update(note);
            await context.SaveChangesAsync(ct);

            logger.LogInformation("Note with ID -> {Id} updated", note.Id);

            return Results.Ok(note);
        });
    }
}
